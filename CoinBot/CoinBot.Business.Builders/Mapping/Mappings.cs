using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoinBot.Business.Builders.Mapping
{
    public class Mappings
    {
        public static void RegisterMappings()
        {
            var all = Assembly
                        .GetEntryAssembly()
                        .GetReferencedAssemblies()
                        .Select(Assembly.Load)
                        .SelectMany(x => x.DefinedTypes)
                        .Where(type => typeof(IProfile).GetTypeInfo().IsAssignableFrom(type.AsType()));

            foreach (var ti in all)
            {
                var t = ti.AsType();
                if (t.Equals(typeof(IProfile)))
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.AddProfiles(t);
                    });
                }
            }
        }
    }
}
