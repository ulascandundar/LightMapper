using LightMapper;

namespace LightMapper.Tests;

public class NestedMappingTests
{
    public class Address
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
    }

    public class AddressDto
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
    }

    public class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public Address? HomeAddress { get; set; }
        public Address? WorkAddress { get; set; }
    }

    public class PersonDto
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public AddressDto? HomeAddress { get; set; }
        public AddressDto? WorkAddress { get; set; }
    }

    [Fact]
    public void Map_WithNestedObject_ShouldMapNestedProperties()
    {
        var person = new Person
        {
            Name = "John Doe",
            Age = 30,
            HomeAddress = new Address
            {
                Street = "123 Main St",
                City = "New York",
                Country = "USA"
            }
        };

        var dto = Mapper.Map<Person, PersonDto>(person);

        Assert.Equal("John Doe", dto.Name);
        Assert.Equal(30, dto.Age);
        Assert.NotNull(dto.HomeAddress);
        Assert.Equal("123 Main St", dto.HomeAddress.Street);
        Assert.Equal("New York", dto.HomeAddress.City);
        Assert.Equal("USA", dto.HomeAddress.Country);
        Assert.Null(dto.WorkAddress);
    }

    [Fact]
    public void Map_WithMultipleNestedObjects_ShouldMapAll()
    {
        var person = new Person
        {
            Name = "Jane Smith",
            Age = 25,
            HomeAddress = new Address
            {
                Street = "456 Oak Ave",
                City = "Los Angeles",
                Country = "USA"
            },
            WorkAddress = new Address
            {
                Street = "789 Business Blvd",
                City = "San Francisco",
                Country = "USA"
            }
        };

        var dto = Mapper.Map<Person, PersonDto>(person);

        Assert.Equal("Jane Smith", dto.Name);
        Assert.NotNull(dto.HomeAddress);
        Assert.Equal("456 Oak Ave", dto.HomeAddress.Street);
        Assert.NotNull(dto.WorkAddress);
        Assert.Equal("789 Business Blvd", dto.WorkAddress.Street);
    }

    [Fact]
    public void Map_WithNullNestedObject_ShouldHandleGracefully()
    {
        var person = new Person
        {
            Name = "Bob Johnson",
            Age = 40,
            HomeAddress = null,
            WorkAddress = null
        };

        var dto = Mapper.Map<Person, PersonDto>(person);

        Assert.Equal("Bob Johnson", dto.Name);
        Assert.Equal(40, dto.Age);
        Assert.Null(dto.HomeAddress);
        Assert.Null(dto.WorkAddress);
    }

    [Fact]
    public void Map_WithNestedObjectAndConfig_ShouldRespectIgnore()
    {
        var config = new MapperConfig();
        config.CreateMap<Person, PersonDto>()
              .Ignore(dest => dest.WorkAddress);

        var person = new Person
        {
            Name = "Alice Brown",
            Age = 35,
            HomeAddress = new Address { Street = "Home St", City = "Home City", Country = "USA" },
            WorkAddress = new Address { Street = "Work St", City = "Work City", Country = "USA" }
        };

        var dto = Mapper.Map<Person, PersonDto>(person, config);

        Assert.Equal("Alice Brown", dto.Name);
        Assert.NotNull(dto.HomeAddress);
        Assert.Equal("Home St", dto.HomeAddress.Street);
        Assert.Null(dto.WorkAddress); // Ignored
    }

    public class Company
    {
        public string Name { get; set; } = "";
        public Address? HeadOffice { get; set; }
    }

    public class CompanyDto
    {
        public string Name { get; set; } = "";
        public AddressDto? HeadOffice { get; set; }
    }

    public class Employee
    {
        public string Name { get; set; } = "";
        public Company? Company { get; set; }
        public Address? HomeAddress { get; set; }
    }

    public class EmployeeDto
    {
        public string Name { get; set; } = "";
        public CompanyDto? Company { get; set; }
        public AddressDto? HomeAddress { get; set; }
    }

    [Fact]
    public void Map_WithDeepNestedObjects_ShouldMapAllLevels()
    {
        var employee = new Employee
        {
            Name = "John Developer",
            Company = new Company
            {
                Name = "Tech Corp",
                HeadOffice = new Address
                {
                    Street = "100 Tech Street",
                    City = "Silicon Valley",
                    Country = "USA"
                }
            },
            HomeAddress = new Address
            {
                Street = "200 Home Ave",
                City = "Hometown",
                Country = "USA"
            }
        };

        var dto = Mapper.Map<Employee, EmployeeDto>(employee);

        Assert.Equal("John Developer", dto.Name);
        Assert.NotNull(dto.Company);
        Assert.Equal("Tech Corp", dto.Company.Name);
        Assert.NotNull(dto.Company.HeadOffice);
        Assert.Equal("100 Tech Street", dto.Company.HeadOffice.Street);
        Assert.Equal("Silicon Valley", dto.Company.HeadOffice.City);
        Assert.NotNull(dto.HomeAddress);
        Assert.Equal("200 Home Ave", dto.HomeAddress.Street);
    }}
