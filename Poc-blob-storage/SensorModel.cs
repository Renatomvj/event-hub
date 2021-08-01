using System;

namespace Poc_blob_storage
{
    internal class SensorModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public object Content { get; set; }
    }
}