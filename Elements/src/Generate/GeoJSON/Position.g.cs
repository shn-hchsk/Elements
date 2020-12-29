//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.1.21.0 (Newtonsoft.Json v11.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------
using Elements;
using Elements.GeoJSON;
using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Properties;
using Elements.Validators;
using Elements.Serialization.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using Line = Elements.Geometry.Line;
using Polygon = Elements.Geometry.Polygon;

namespace Elements.GeoJSON
{
    #pragma warning disable // Disable all warnings

    /// <summary>A position.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.21.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Position 
    {
        [Newtonsoft.Json.JsonConstructor]
        public Position(double @latitude, double @longitude)
        {
            var validator = Validator.Instance.GetFirstValidatorForType<Position>();
            if(validator != null)
            {
                validator.PreConstruct(new object[]{ @latitude, @longitude});
            }
        
            this.Latitude = @latitude;
            this.Longitude = @longitude;
            
            if(validator != null)
            {
                validator.PostConstruct(this);
            }
        }
    
        /// <summary>The latitude in decimal degrees.</summary>
        [Newtonsoft.Json.JsonProperty("latitude", Required = Newtonsoft.Json.Required.Always)]
        public double Latitude { get; set; }
    
        /// <summary>The longitude in decimal degrees.</summary>
        [Newtonsoft.Json.JsonProperty("longitude", Required = Newtonsoft.Json.Required.Always)]
        public double Longitude { get; set; }
    
    
    }
}