using System;
using Declarations.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoBannersContext.Documents
{
    public class BannerDoc : IBanner
    {
        // todo: validators while writing duplicate id
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public int Id { get; set; }
        public string Html { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Created { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Modified { get; set; }
    }
}
