using Mapster;

namespace Meteor.Sessions.Infrastructure.Mapping;

public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Grpc.Customer, Core.Models.Customer>()
            .Map(coreModel => coreModel.Active, grpcModel => grpcModel.Status == Grpc.CUSTOMER_STATUS.Active);

        config.ForType<Grpc.Employee, Core.Models.Employee>()
            .Map(
                coreModel => coreModel.Active,
                grpcModel => grpcModel.Status == Grpc.EMPLOYEE_STATUS.Active
                             || grpcModel.Status == Grpc.EMPLOYEE_STATUS.OnLeave
            );
    }
}