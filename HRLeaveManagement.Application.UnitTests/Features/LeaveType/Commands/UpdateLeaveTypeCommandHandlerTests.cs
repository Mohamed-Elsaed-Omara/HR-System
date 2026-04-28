using AutoMapper;
using HRLeaveManagement.Application.Contracts.Logging;
using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Application.Features.LeaveType.Commands.UpdateLeaveType;
using HRLeaveManagement.Application.MappingProfiles;
using HRLeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HRLeaveManagement.Application.UnitTests.Features.LeaveType.Commands
{
    public class UpdateLeaveTypeCommandHandlerTests
    {
        private Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<UpdateLeaveTypeCommandHandler>> _mockAppLogger;

        public UpdateLeaveTypeCommandHandlerTests()
        {
            _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<LeaveTypeProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _mockAppLogger = new Mock<IAppLogger<UpdateLeaveTypeCommandHandler>>();
        }

        [Fact]
        public async Task ShouldUpdateLeaveType()
        {
            // Arrange
            var handler = new UpdateLeaveTypeCommandHandler(_mapper,_mockRepo.Object, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(new UpdateLeaveTypeCommand() { Id = 1, Name = "Updated", DefaultDays = 30 },
                CancellationToken.None);
            var updated = await _mockRepo.Object.GetByIdAsync(1);

            // Assert
            updated.Name.ShouldBe("Updated");
            updated.DefaultDays.ShouldBe(30);
        }
    }
}
