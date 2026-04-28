using AutoMapper;
using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;
using HRLeaveManagement.Application.MappingProfiles;
using HRLeaveManagement.Application.UnitTests.Mocks;
using HRLeaveManagement.Domain;
using Moq;
using Shouldly;

namespace HRLeaveManagement.Application.UnitTests.Features.LeaveType.Commands
{
    public class CreateleaveTypeCommandTests
    {
        private Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;

        public CreateleaveTypeCommandTests() 
        {
            _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<LeaveTypeProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task ShouldCreateLeaveType()
        {
            // Arrange
            var handler = new CreateLeaveTypeCommandHandler(_mapper, _mockRepo.Object);

            // Act
            var result = await handler.Handle(new CreateLeaveTypeCommand() { Name = "Test", DefaultDays = 1 },
                CancellationToken.None);

            // Assert
            var leaveTypes = await _mockRepo.Object.GetAsync();
            leaveTypes.Count.ShouldBe(4);

        }
    }
}
