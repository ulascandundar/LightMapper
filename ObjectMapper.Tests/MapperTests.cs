using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightMapper;
namespace ObjectMapper.Tests;
public class MapperConfigTests
{
	public class Person
	{
		public string Name { get; set; } = "";
		public int Age { get; set; }
		public string Secret { get; set; } = "";
	}

	public class PersonDto
	{
		public string FullName { get; set; } = "";
		public int Age { get; set; }
		public string Secret { get; set; } = "";
	}

	[Fact]
	public void Map_WithForMember_ShouldMapDifferentPropertyNames()
	{
		var config = new MapperConfig();
		config.CreateMap<Person, PersonDto>()
			  .ForMember(dest => dest.FullName, src => src.Name);

		var person = new Person { Name = "Ulaş", Age = 30, Secret = "hidden" };
		var dto = Mapper.Map<Person, PersonDto>(person, config);

		Assert.Equal("Ulaş", dto.FullName);
		Assert.Equal(30, dto.Age);
		Assert.Equal("hidden", dto.Secret); // otomatik mapleniyor çünkü ignore değil
	}

	[Fact]
	public void Map_WithIgnore_ShouldNotSetIgnoredProperty()
	{
		var config = new MapperConfig();
		config.CreateMap<Person, PersonDto>()
			  .ForMember(dest => dest.FullName, src => src.Name)
			  .Ignore(dest => dest.Secret);

		var person = new Person { Name = "Ulaş", Age = 30, Secret = "1234" };
		var dto = Mapper.Map<Person, PersonDto>(person, config);

		Assert.Equal("Ulaş", dto.FullName);
		Assert.Equal(30, dto.Age);
		Assert.Equal("", dto.Secret); // Ignore edildiği için default değer
	}

	[Fact]
	public void Map_WithoutConfig_ShouldFallbackToPropertyNameMatching()
	{
		var person = new Person { Name = "Ulaş", Age = 30, Secret = "xyz" };

		var dto = Mapper.Map<Person, PersonDto>(person);

		Assert.Equal("", dto.FullName); // FullName eşleşmediği için boş kalmalı
		Assert.Equal(30, dto.Age);
		Assert.Equal("xyz", dto.Secret);
	}

	[Fact]
	public void Map_ShouldThrow_WhenSourceIsNull()
	{
		Person? person = null;
		var config = new MapperConfig();

		Assert.Throws<ArgumentNullException>(() => Mapper.Map<Person, PersonDto>(person!, config));
	}
}