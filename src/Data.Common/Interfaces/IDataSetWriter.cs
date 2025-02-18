using SqlBuildingBlocks.POCOs;

namespace Data.Common.Interfaces;

public interface IDataSetWriter
{
    void WriteDataSet(VirtualDataSet dataSet);
}
