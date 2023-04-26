using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Meteor.Sessions.Api.Mapping;

public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Core.Models.Session, Grpc.Session>()
            .Map(grpcModel => grpcModel.CreateDate, coreModel => Timestamp.FromDateTimeOffset(coreModel.CreateDate))
            .Map(grpcModel => grpcModel.ExpireDate, coreModel => Timestamp.FromDateTimeOffset(coreModel.ExpireDate))
            .Map(grpcModel => grpcModel.LastRefreshDate, coreModel => Timestamp.FromDateTimeOffset(coreModel.LastRefreshDate));
    }
}