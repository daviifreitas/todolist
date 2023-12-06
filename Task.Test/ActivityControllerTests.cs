using Activity.API.ApiModel;
using Activity.API.Controllers;
using Activity.API.Entities;
using Activity.API.Interfaces;
using AutoFixture;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Activity.Test
{
    public class ActivityControllerTests
    {
        private readonly IActivityTicketRepository _activityTicketRepository;
        private readonly ActivityTicketController _activityController;
        private readonly IConfiguration _config;
        private readonly IFixture _fixture;
        private readonly Faker _faker = new Faker();


        public ActivityControllerTests()
        {
            _activityTicketRepository = A.Fake<IActivityTicketRepository>();
            _config = A.Fake<IConfiguration>();
            _fixture = new Fixture();
            _activityController = new ActivityTicketController(_activityTicketRepository, _config);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsOkObjectResult_WithNull()
        {
            A.CallTo(() => _activityTicketRepository.GetByIdAsync(A<int>.Ignored))
                .Returns(null as ActivityTicket);

            IActionResult result = await _activityController.GetById(1);

            if (result is OkObjectResult okResult)
            {
                okResult.Value.Should().BeNull();
            }
            else
            {
                Assert.True(false, "Expected OkObjectResult");
            }

        }

        [Fact]
        public async Task Create_WithValidModel_ReturnsOkObjectResult_WithMessage()
        {

            var model = new ActivityTicketApiModel();
            var expectedMessage = "ActivityTicketTicket created successfully";


            A.CallTo(() => _activityTicketRepository.AddAsync(A<ActivityTicket>.Ignored))
                .Returns(true);

            var result = await _activityController.Create(model);


            if (result is OkObjectResult okResult)
            {

                okResult.Value.Should().BeEquivalentTo(expectedMessage);
            }
            else
            {
                Assert.True(false, "Expected OkObjectResult");
            }
        }

        [Fact]
        public async void Create_With_Any_Exception_Dont_Throws()
        {
            ActivityTicketApiModel activityTicketForCreateForApi = _fixture.Create<ActivityTicketApiModel>();

            A.CallTo(() => _activityTicketRepository.AddAsync(A<ActivityTicket>.Ignored))
                .Throws(new Exception());

            // create a assert about dont throws exception 
            Exception? threwApiException = await Record.ExceptionAsync(async () => await _activityController.Create(activityTicketForCreateForApi));
            threwApiException.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void Create_With_Valid_Data_Should_Created_With_Sucessfull(bool createdActivityResult)
        {
            ActivityTicketApiModel currentActivityTicketApiModelForCreate = _fixture.Create<ActivityTicketApiModel>();

            A.CallTo(() => _activityTicketRepository.AddAsync(A<ActivityTicket>.Ignored))
                .Returns(createdActivityResult);

            IActionResult result = await _activityController.Create(currentActivityTicketApiModelForCreate);

            if (result is OkObjectResult resultAsOkObjectResult)
            {
                resultAsOkObjectResult.Value.Should().BeEquivalentTo("ActivityTicketTicket created successfully");
            }
            else if (result is BadRequestObjectResult resultAsBadRequestObjectResult)
            {
                resultAsBadRequestObjectResult.Value.Should().BeEquivalentTo("ActivityTicketTicket creation failed");
            }
            else
            {
                Assert.True(false, "Expected OkObjectResult or BadRequestObjectResult");
            }
        }

        [Fact]
        public async void Edit_Dont_Exist_ActivityTicket_Dont_Be_Updated()
        {

            ActivityTicketApiModel activityTicketApiModelForUpdate = _fixture.Create<ActivityTicketApiModel>();
            int activityTIcketIdForUpdate = _faker.Random.Int();
            ActivityTicket? entityForUpdate = null;

            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTIcketIdForUpdate))
                .Returns(entityForUpdate);

            await _activityController.Update(activityTIcketIdForUpdate, activityTicketApiModelForUpdate);

            A.CallTo(() => _activityTicketRepository.UpdateAsync(A<ActivityTicket>.Ignored))
                .MustNotHaveHappened();

        }

        [Fact]
        public async void Edit_Has_ActivityTicket_For_Update_Should_Updated_Then_With_Expected_Data()
        {
            ActivityTicketApiModel activityTicketApiModelForUpdateEntity = _fixture.Create<ActivityTicketApiModel>();

            int activityTicketIdForUpdate = _faker.Random.Int();
            ActivityTicket activityTicketEntityByIdForUpdate = _fixture.Create<ActivityTicket>();
            ActivityTicket expectedUpdatedEntity = activityTicketEntityByIdForUpdate;
            activityTicketApiModelForUpdateEntity.UpdateEntity(expectedUpdatedEntity);


            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTicketIdForUpdate))
                .Returns(activityTicketEntityByIdForUpdate);

            await _activityController.Update(activityTicketIdForUpdate, activityTicketApiModelForUpdateEntity);

            A.CallTo(() => _activityTicketRepository
                                .UpdateAsync(A<ActivityTicket>.That
                                        .Matches(updatedActivityTicket =>
                                        updatedActivityTicket.Requester.Equals(expectedUpdatedEntity.Requester) &&
                                        updatedActivityTicket.Assigned.Equals(expectedUpdatedEntity.Assigned) &&
                                        updatedActivityTicket.Description.Equals(expectedUpdatedEntity.Description) &&
                                        updatedActivityTicket.DueDate.Equals(expectedUpdatedEntity.DueDate) &&
                                        updatedActivityTicket.Title.Equals(expectedUpdatedEntity.Title))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void Edit_Has_Any_Exception_Throws_Dont_Throws_Exception()
        {
            ActivityTicketApiModel activityTicketApiModelForUpdateEntity = _fixture.Create<ActivityTicketApiModel>();
            int activityTicketIdForUpdate = _faker.Random.Int();

            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTicketIdForUpdate))
                .Throws(new Exception());

            await _activityController.Update(activityTicketIdForUpdate, activityTicketApiModelForUpdateEntity);

            Exception? threwApiException = await Record.ExceptionAsync(async () => await _activityController.Update(activityTicketIdForUpdate, activityTicketApiModelForUpdateEntity));
            threwApiException.Should().BeNull();
        }

        [Fact]
        public async void Delete_Dont_Exist_ActivityTicket_For_Delete_Dont_Be_Deleted()
        {
            int activityTicketIdForDelete = _faker.Random.Int();
            ActivityTicket? activityTicketById = null;
            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTicketIdForDelete)).Returns(activityTicketById);

            await _activityController.Delete(activityTicketIdForDelete);

            A.CallTo(() => _activityTicketRepository.DeleteAsync(activityTicketIdForDelete)).MustNotHaveHappened();
        }

        [Fact]
        public async void Delete_Has_ActivityTicket_For_Remove_Should_Be_Deleted()
        {
            int activityTicketIdForDelete = _faker.Random.Int();
            ActivityTicket? activityTicketById = _fixture.Create<ActivityTicket>();
            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTicketIdForDelete)).Returns(activityTicketById);

            await _activityController.Delete(activityTicketIdForDelete);

            A.CallTo(() => _activityTicketRepository.DeleteAsync(activityTicketIdForDelete)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void Delete_Throws_Any_Exception_Dont_Threw_To_Another_Layers()
        {
            int activityTicketIdForDelete = _faker.Random.Int();
            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTicketIdForDelete)).ThrowsAsync(new Exception());

            await _activityController.Delete(activityTicketIdForDelete);
            Exception? threwApiException = await Record.ExceptionAsync(async () => await _activityController.Delete(activityTicketIdForDelete));
            threwApiException.Should().BeNull();
        }


        [Fact]
        public async void GetAll_With_Any_Exception_Dont_Throws()
        {
            A.CallTo(() => _activityTicketRepository.GetAllAsync()).Throws(new Exception());

            Exception? threwApiException = await Record.ExceptionAsync(async () => await _activityController.GetAll());
            threwApiException.Should().BeNull();
        }

        [Fact]
        public async void GetAll_With_CorretData_should_Return_ExpectedResult()
        {
            List<ActivityTicket> activitiesFromDb = _fixture.CreateMany<ActivityTicket>().ToList();

            A.CallTo(() => _activityTicketRepository.GetAllAsync()).Returns(activitiesFromDb);

            IActionResult result = await _activityController.GetAll();

            if (result is OkObjectResult resultAsObjectResult)
            {
                resultAsObjectResult.Value.Should().BeEquivalentTo(activitiesFromDb);
            }

        }

        [Fact]
        public async void GetById_With_Any_Exception_Dont_Throws()
        {
            int activityTicketIdForGetById = _faker.Random.Int();
            A.CallTo(() => _activityTicketRepository.GetByIdAsync(activityTicketIdForGetById)).Throws(new Exception());

            Exception? threwApiException = await Record.ExceptionAsync(async () => await _activityController.GetById(activityTicketIdForGetById));
            threwApiException.Should().BeNull();
        }

        [Fact]
        public async void GetById_Existence_Entity_Should_Return_Expected_Data()
        {
            int idForGetActivityTicket = _faker.Random.Int();
            ActivityTicket activityTicketById = _fixture.Create<ActivityTicket>();

            A.CallTo(() => _activityTicketRepository.GetByIdAsync(idForGetActivityTicket)).Returns(activityTicketById);

            IActionResult result = await _activityController.GetById(idForGetActivityTicket);

            if (result is OkObjectResult resultAsOkObjectResult)
            {
                resultAsOkObjectResult.Value.Should().BeEquivalentTo(activityTicketById);
            }
        }


        [Fact]
        public async void GetById_Dont_Exist_With_Null_Result()
        {
            int idForGetActivityTicket = _faker.Random.Int();
            ActivityTicket? activityTicketById = null;

            A.CallTo(() => _activityTicketRepository.GetByIdAsync(idForGetActivityTicket)).Returns(activityTicketById);

            IActionResult result = await _activityController.GetById(idForGetActivityTicket);

            if (result is OkObjectResult resultAsOkObjectResult)
            {
                resultAsOkObjectResult.Value.Should().BeEquivalentTo(activityTicketById);
            }
        }
    }
}
