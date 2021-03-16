﻿using AltV.Net.Async;
using Autofac;
using PARADOX_RP.Core.Interface;
using PARADOX_RP.Core.Module;
using PARADOX_RP.UI;
using PARADOX_RP.UI.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PARADOX_RP.Core.Factories
{
    internal class PXContainer : IDisposable
    {
        private readonly Type[] _loadedTypes = Assembly.GetExecutingAssembly().GetTypes();

        private IContainer _container;
        private ILifetimeScope _scope;

        private List<Type> _handlerTypes = new List<Type>();
        private List<Type> _moduleTypes = new List<Type>();
        private List<Type> _itemTypes = new List<Type>();
        private List<Type> _windowTypes = new List<Type>();

        internal void RegisterTypes()
        {
            var builder = new ContainerBuilder();

            LogStartup("Load types");
            LoadTypes();

            builder.RegisterType<CoreSystem>().As<ICoreSystem>();
            builder.RegisterType<WindowManager>().As<IWindowManager>();

            LogStartup("Register handlers");
            foreach (var handler in _handlerTypes)
            {
                builder.RegisterTypes(handler)
                .AsImplementedInterfaces()
                .SingleInstance();
            }

            LogStartup("Register modules");
            foreach (var module in _moduleTypes)
            {
                builder.RegisterType(module)
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            }

            LogStartup("Register windows");
            foreach (var window in _windowTypes)
            {
                builder.RegisterType(window)
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            }

            //LogStartup("Register itemscripts");
            //foreach (var item in _itemTypes)
            //{
            //    builder.RegisterType(item)
            //    .As<IItemScript>()
            //    .SingleInstance();
            //}

            //LogStartup("Register database context");
            //var dbContextOptionsBuilder = new DbContextOptionsBuilder<RPContext>()
            //    .UseMySql("server=db.sibauirp.de;database=sibauirp;user=Sibaui;password=XPYfMKEUMN9wqXcS!yDtHAw4qc?Nh?Bz3wF7r-SxgnXQ-q8Yy-faDMd8F7C_8BV6;treattinyasboolean=true",
            //        b => b.ServerVersion("10.4.11-mariadb")
            //            .EnableRetryOnFailure());


            //dotnet ef dbcontext scaffold "server=db.sibauirp.de;database=sibauirp;user=Sibaui;password=XPYfMKEUMN9wqXcS!yDtHAw4qc?Nh?Bz3wF7r-SxgnXQ-q8Yy-faDMd8F7C_8BV6;treattinyasboolean=true" "Pomelo.EntityFrameworkCore.MySql" -o Models -c RPContext -f

            //builder.RegisterType<RPContext>()
            //   .WithParameter("options", dbContextOptionsBuilder.Options)
            //   .InstancePerLifetimeScope();

            _container = builder.Build();
        }

        internal void ResolveTypes()
        {
            _scope = _container.BeginLifetimeScope();
            foreach (var type in _moduleTypes)
            {
                _scope.Resolve(type);
            }

            foreach (var type in _windowTypes)
            {
                _scope.Resolve(type);
            }
        }

        internal T Resolve<T>()
        {
            return _scope.Resolve<T>();
        }

        private void LoadTypes()
        {
            foreach (Type type in _loadedTypes)
            {
                if (IsHandlerType(type))
                {
                    _handlerTypes.Add(type);
                }
                else if (IsModuleType(type))
                {
                    _moduleTypes.Add(type);
                }
                else if (IsItemType(type))
                {
                    _itemTypes.Add(type);
                }
                else if (IsWindowType(type))
                {
                    _windowTypes.Add(type);
                }
            }
        }

        private bool IsHandlerType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("PARADOX_RP.Handlers") &&
                                            !type.Name.StartsWith("<");
        }

        private bool IsModuleType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("PARADOX_RP.Game") &&
                                            type.BaseType != null &&
                                            (type.BaseType == typeof(ModuleBase) ||
                                            type.BaseType.IsGenericType) &&
                                            !type.Name.StartsWith("<");
        }

        private bool IsWindowType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("PARADOX_RP.UI") &&
                                            type.BaseType != null &&
                                            (type.BaseType == typeof(Window) ||
                                            type.BaseType.IsGenericType) &&
                                            !type.Name.StartsWith("<");
        }

        private bool IsItemType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("GangRP_Server.Modules.Inventory.Item") &&
                                            !type.Name.StartsWith("<");
        }

        private static void LogStartup(string text)
        {
            AltAsync.Log($"[STARTUP] {text}");
        }

        public void Dispose()
        {
            _container.Dispose();
            _scope.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}