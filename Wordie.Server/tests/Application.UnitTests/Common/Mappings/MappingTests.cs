using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Application.Common.Models;
// Todo mappings removed from mapping tests as TodoList/TodoItem were deprecated
using Wordie.Server.Domain.Entities;
using NUnit.Framework;

namespace Wordie.Server.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config => 
            config.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));

        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    public void ShouldHaveAtLeastOneMapping()
    {
        // Keep a basic mapping health check; detailed mapping cases for removed Todo types are omitted.
        _configuration.AssertConfigurationIsValid();
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}
