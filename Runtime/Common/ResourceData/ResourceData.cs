using System;
using Newtonsoft.Json;
using Runtime.Definition;

namespace Runtime.Common.Resource
{
    [Serializable]
    public struct ResourceData
    {
        #region Members

        [JsonProperty("resource_type")]
        public ResourceType resourceType;

        [JsonProperty("resource_id")]
        public int resourceId;

        [JsonProperty("resource_number")]
        public long resourceNumber;

        #endregion Members

        #region Struct Methods

        public ResourceData(ResourceType resourceType, int resourceId, long resourceNumber)
        {
            this.resourceType = resourceType;
            this.resourceId = resourceId;
            this.resourceNumber = resourceNumber;
        }

        public ResourceData Multiply(float multiplier)
        {
            return new ResourceData(resourceType, resourceId, (long)(resourceNumber * multiplier));
        }

        #endregion Struct Methods
    }
}