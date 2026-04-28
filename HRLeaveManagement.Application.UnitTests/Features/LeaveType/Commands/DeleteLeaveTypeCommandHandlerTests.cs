using AutoMapper;
using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Application.Features.LeaveType.Commands.DeleteLeaveType;
using HRLeaveManagement.Application.MappingProfiles;
using HRLeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HRLeaveManagement.Application.UnitTests.Features.LeaveType.Commands
{
    public class DeleteLeaveTypeCommandHandlerTests
    {
        private Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;

        public DeleteLeaveTypeCommandHandlerTests()
        {
            _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

            //var mapperConfig = new MapperConfiguration(c =>
            //{
            //    c.AddProfile<LeaveTypeProfile>();
            //});

            //_mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task ShouldDeleteLeaveType()
        {
            // Arrange
            var handler = new DeleteLeaveTypeCommandHandler(_mockRepo.Object);

            // Act
            await handler.Handle(new DeleteLeaveTypeCommand() { Id = 1 }, CancellationToken.None);

            var result = await _mockRepo.Object.GetByIdAsync(1);

            result.ShouldBeNull();
        }
    }
}
