using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.Interfaces;

internal interface IContainsReturning
{
    int? Returning { get; }
}
