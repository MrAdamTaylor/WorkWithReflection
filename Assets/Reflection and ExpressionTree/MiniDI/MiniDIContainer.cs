using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection_and_ExpressionTree.MiniDI
{
    public class MiniDIContainer 
    {
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Dictionary<Type, Delegate> _constructDelegates = new();
        private readonly Dictionary<Type, Delegate> _fieldPropertyInjectors = new();

        // Регистрируем уже созданный экземпляр
        public void Register<T>(T instance)
        {
            _instances[typeof(T)] = instance;
        }

        // Получаем экземпляр по типу
        public T Resolve<T>()
        {
            return (T)_instances[typeof(T)];
        }

        // Внедряем зависимости через метод Construct
        public void InjectConstruct<T>()
        {
            var type = typeof(T);
            if (!_instances.TryGetValue(type, out var instance))
                throw new Exception($"Type {type} not registered!");

            if (!_constructDelegates.TryGetValue(type, out var del))
            {
                del = CreateConstructDelegate(type);
                _constructDelegates[type] = del;
            }

            del.DynamicInvoke(instance, this);
        }
        
        public void InjectFieldsAndProperties<T>()
        {
            var type = typeof(T);
            if (!_instances.TryGetValue(type, out var instance))
                throw new Exception($"Type {type} not registered!");

            if (!_fieldPropertyInjectors.TryGetValue(type, out var del))
            {
                del = CreateInjectFieldsAndPropertiesDelegate(type);
                _fieldPropertyInjectors[type] = del;
            }

            del.DynamicInvoke(instance, this);
        }
        
        public void AutoRegister(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract) continue;
                if (!type.IsDefined(typeof(AutoRegisterAttribute), false)) continue;

                if (_instances.ContainsKey(type)) continue;

                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null) continue;

                var instance = ctor.Invoke(null);
                _instances[type] = instance!;
            }
        }

        // Создаём делегат конструктора через Expression Tree
        private Delegate CreateConstructDelegate(Type type)
        {
            var method = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
                throw new Exception($"No Construct method found in {type}");

            var parameters = method.GetParameters();

            // Аргументы Expression
            var instanceParam = Expression.Parameter(type, "instance");
            var containerParam = Expression.Parameter(typeof(MiniDIContainer), "container");

            var paramExpressions = new List<Expression>();
            foreach (var p in parameters)
            {
                // container.Resolve<ParameterType>()
                var resolveCall = Expression.Call(
                    containerParam,
                    nameof(Resolve),
                    new[] { p.ParameterType }
                );
                paramExpressions.Add(resolveCall);
            }

            // instance.Construct(arg1, arg2, ...)
            var call = Expression.Call(instanceParam, method, paramExpressions);

            // (T instance, Container container) => instance.Construct(container.Resolve<T>())
            var lambdaType = typeof(Action<,>).MakeGenericType(type, typeof(MiniDIContainer));
            return Expression.Lambda(lambdaType, call, instanceParam, containerParam).Compile();
        }
        
        private Delegate CreateInjectFieldsAndPropertiesDelegate(Type type)
{
    var instanceParam = Expression.Parameter(type, "instance");
    var containerParam = Expression.Parameter(typeof(MiniDIContainer), "container");

    var expressions = new List<Expression>();

    // Поля
    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
    {
        if (!field.IsDefined(typeof(InjectAttribute)) && !field.IsDefined(typeof(OptionalInjectAttribute)))
            continue;

        bool isOptional = field.IsDefined(typeof(OptionalInjectAttribute));

        var keyType = Expression.Constant(field.FieldType, typeof(Type));

        // container._instances.TryGetValue(fieldType, out object result)
        var tryGetValueMethod = typeof(Dictionary<Type, object>).GetMethod("TryGetValue")!;
        var instancesField = Expression.Field(containerParam, "_instances");
        var resultVar = Expression.Variable(typeof(object), "resolved");

        var tryGetCall = Expression.Call(instancesField, tryGetValueMethod, keyType, resultVar);

        // ((FieldType)result)
        var castedResult = Expression.Convert(resultVar, field.FieldType);

        // Присвоение поля
        var assignExpr = Expression.Assign(Expression.Field(instanceParam, field), castedResult);

        Expression assignBlock = isOptional
            ? Expression.IfThen(tryGetCall, assignExpr) // Только если есть в контейнере
            : Expression.Block(
                Expression.IfThenElse(
                    tryGetCall,
                    assignExpr,
                    Expression.Throw(
                        Expression.New(typeof(Exception).GetConstructor(new[] { typeof(string) })!,
                        Expression.Constant($"Required dependency '{field.FieldType}' not registered for field '{field.Name}' in {type.Name}"))
                    )
                )
            );

        expressions.Add(resultVar);
        expressions.Add(assignBlock);
    }

    // Свойства
    foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
        if ((!prop.IsDefined(typeof(InjectAttribute)) && !prop.IsDefined(typeof(OptionalInjectAttribute))) || !prop.CanWrite)
            continue;

        bool isOptional = prop.IsDefined(typeof(OptionalInjectAttribute));

        var keyType = Expression.Constant(prop.PropertyType, typeof(Type));
        var tryGetValueMethod = typeof(Dictionary<Type, object>).GetMethod("TryGetValue")!;
        var instancesField = Expression.Field(containerParam, "_instances");
        var resultVar = Expression.Variable(typeof(object), "resolved");

        var tryGetCall = Expression.Call(instancesField, tryGetValueMethod, keyType, resultVar);
        var castedResult = Expression.Convert(resultVar, prop.PropertyType);
        var assignExpr = Expression.Assign(Expression.Property(instanceParam, prop), castedResult);

        Expression assignBlock = isOptional
            ? Expression.IfThen(tryGetCall, assignExpr)
            : Expression.Block(
                Expression.IfThenElse(
                    tryGetCall,
                    assignExpr,
                    Expression.Throw(
                        Expression.New(typeof(Exception).GetConstructor(new[] { typeof(string) })!,
                        Expression.Constant($"Required dependency '{prop.PropertyType}' not registered for property '{prop.Name}' in {type.Name}"))
                    )
                )
            );

        expressions.Add(resultVar);
        expressions.Add(assignBlock);
    }

    var variables = expressions.OfType<ParameterExpression>().ToList();
    var body = Expression.Block(variables, expressions);
    var lambdaType = typeof(Action<,>).MakeGenericType(type, typeof(MiniDIContainer));
    return Expression.Lambda(lambdaType, body, instanceParam, containerParam).Compile();
}
    }
}
