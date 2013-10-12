using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Db.Services.Controllers
{
    public class ValuesController : BaseApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(
          () =>
          {
              if (true)
              {
                  throw new ArgumentNullException("Some exeption");
              }

              return new string[] { "value1", "value2" };
          });

            return responseMsg;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}