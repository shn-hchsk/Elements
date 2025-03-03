using Elements.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Color = Elements.Geometry.Color;

namespace Elements.Tests
{
    public class MaterialTests : ModelTest
    {
        [Fact, Trait("Category", "Examples")]
        public void Example()
        {
            this.Name = "Elements_Material";

            // <example>
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            var specularFactor = 0.0;
            var glossinessFactor = 0.0;

            var textData = new List<(Vector3, Vector3, Vector3, string, Color?)>();

            var sphere = Mesh.Sphere(0.5, 20);

            for (var r = 0.0; r <= 1.0; r += 0.2)
            {
                for (var g = 0.0; g <= 1.0; g += 0.2)
                {
                    if (r == 0)
                    {
                        textData.Add((new Vector3(-1.5, y), Vector3.ZAxis, Vector3.XAxis, $"roughness: {1 - glossinessFactor:f2}", Colors.Black));
                    }
                    if (g == 0)
                    {
                        textData.Add((new Vector3(x, -1), Vector3.ZAxis, Vector3.XAxis, $"specular: {specularFactor:f2}", Colors.Black));
                    }
                    for (var b = 0.0; b <= 1.0; b += 0.2)
                    {
                        var color = new Color(r, g, b, 1 - b);
                        if (r == 1.0 && g == 0.0)
                        {
                            textData.Add((new Vector3(x + 1.5, y, z), Vector3.YAxis.Negate(), Vector3.XAxis, $"alpha: {color.Alpha:f2}", Colors.Black));
                        }
                        var material = new Material($"{r}_{g}_{b}", color, specularFactor, glossinessFactor);
                        Model.AddElement(new MeshElement(sphere, new Transform(new Vector3(x, y, z)), material));
                        z += 2.0;
                    }
                    z = 0;
                    y += 2.0;
                    glossinessFactor += 0.2;
                }
                glossinessFactor = 0.0;
                y = 0;
                x += 2.0;
                specularFactor += 0.2;
            }
            Model.AddElement(new ModelText(textData, FontSize.PT72));
            // </example>
        }

        [Fact]
        public void Construct()
        {
            var material = new Material("test", new Color(1.0f, 1.0f, 1.0f, 1.0f), 1.0f, 1.0f);
            Assert.NotNull(material);
            Assert.Equal(1.0f, material.Color.Red);
            Assert.Equal(1.0f, material.Color.Blue);
            Assert.Equal(1.0f, material.Color.Green);
            Assert.Equal(1.0f, material.Color.Alpha);
            Assert.Equal(1.0f, material.SpecularFactor);
            Assert.Equal(1.0f, material.GlossinessFactor);
        }

        [Fact]
        public void InvalidValues_Construct_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Material("test", new Color(-1.0f, 1, 0f, 1.0f), 1.0f, 1.0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Material("test", new Color(3.0f, 1, 0f, 1.0f), 1.0f, 1.0f));
        }

        [Fact]
        public void StaticColor()
        {
            var material = new Material("test", Colors.Mint, 0.2f, 0.2f);
            Assert.Equal(material.Color, Colors.Mint);
        }

        [Fact]
        public void TextureTest()
        {
            this.Name = "TextureTest";
            var m = new Material("test", Colors.Gray, 0.0f, 0.0f, "./Textures/UV.jpg", true);
            var mass = new Mass(new Circle(Vector3.Origin, 5).ToPolygon(), 10, m);
            this.Model.AddElement(mass);
        }

        [Fact]
        public void NormalTextureTest()
        {
            this.Name = "NormalTextureTest";
            var m = new Material("test", Colors.Sand, 0.5f, 0.5f)
            {
                NormalTexture = "./Textures/UV.jpg"
            };
            var mass = new Mass(new Circle(Vector3.Origin, 5).ToPolygon(), 10, m);
            this.Model.AddElement(mass);
        }

        [Fact]
        public void EmissiveTextureTest()
        {
            Name = nameof(EmissiveTextureTest);

            var m = new Material("test", Colors.Orange, emissiveTexture: "./Textures/Checkerboard.png", emissiveFactor: 0.5);
            var sphere = Mesh.Sphere(2, 30);
            Model.AddElement(new MeshElement(sphere, material: m));
        }

        [Fact]
        public void BuiltInMaterialsAlwaysHaveSameId()
        {
            var m1 = BuiltInMaterials.Wood;
            this.Model.AddElement(m1);
            var json = this.Model.ToJson();
            var newModel = Model.FromJson(json);
            var m2 = newModel.AllElementsOfType<Material>().First();
            Assert.Equal(m1.Id, m2.Id);
        }

        [Fact]
        public void ImplicitConversion()
        {
            var material = new Material("A test", (0.5, 1, 0.2));

            Assert.Equal(new Color(0.9, 0.3, 0.5, 1.0), (0.9, 0.3, 0.5));
        }
    }
}