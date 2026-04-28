using AutoMapper;
using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
using HRLeaveManagement.Application.MappingProfiles;
using HRLeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HRLeaveManagement.Application.UnitTests.Features.LeaveType.Queries
{
    public class GetLeaveTypeDetailsQueryHandlerTests
    {
        private Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;

        public GetLeaveTypeDetailsQueryHandlerTests() 
        {
            _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<LeaveTypeProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task ShouldReturnLeaveTypeDetails()
        {
            // Arrange
            var handler = new GetLeaveTypeDetailsQueryHandler(_mapper,_mockRepo.Object);

            // Act
            var result = await handler.Handle(new GetLeaveTypeDetailsQuery(1), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }
    }
}
