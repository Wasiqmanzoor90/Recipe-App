using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WebApplication1.Interface
{
    public interface ITokenService
    {
        string CreateToken(ObjectId id, string Email);
         ObjectId VerifyToken(string id);
        
    }
}