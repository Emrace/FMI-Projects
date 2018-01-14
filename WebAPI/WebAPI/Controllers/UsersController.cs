using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebAPI.Classes;
using WebAPI.Filters;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class UsersController : ApiController
    {
        private readonly UserRepository _repository;

        public UsersController(UserRepository repo)
        {
            _repository = repo;
        }

        [HttpPost]
        [Route("api/logins")]
        public HttpResponseMessage LogIn([FromBody]User user)
        {
            User resultUser = _repository.Get(user.Username);

            User userToReturn = new User
            {
                AuthToken = resultUser.AuthToken,
                Id = resultUser.Id,
                Username = resultUser.Username
            };

            return Request.CreateResponse(HttpStatusCode.OK, userToReturn);
        }

        [HttpDelete]
        [UserAuthorizationFilter]
        [Route("api/logins/{id:int}")]
        public HttpResponseMessage LogOut(int id)
        {
            _repository.Get(id).AuthToken = null;

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        // GET: api/Users
        [HttpGet]
        [UserAuthorizationFilter]
        [Route("api/Users")]
        public HttpResponseMessage Get()
        {
            List<UserWrapper> result = _repository.GetAll().Select(user => new UserWrapper(user)).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // GET: api/Users/5
        [HttpGet]
        [UserAuthorizationFilter]
        [Route("api/Users/{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            User user = _repository.Get(id);

            UserWrapper result = new UserWrapper
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // POST: api/Users
        [HttpPost]
        [Route("api/Users")]
        public HttpResponseMessage Post([FromBody]User newUser)
        {
            UserWrapper resultUser = new UserWrapper(_repository.Create(newUser));

            return Request.CreateResponse(HttpStatusCode.OK, resultUser);
        }

        // PUT: api/Users/5
        [Route("api/Users/{id:int}")]
        [UserAuthorizationFilter]
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody]UserUpdateRequest editedUser)
        {
            UserWrapper result = _repository.Update(id, editedUser);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // DELETE: api/Users/5
        [HttpDelete]
        [UserAuthorizationFilter]
        [Route("api/Users/{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            _repository.Delete(id);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [UserAuthorizationFilter]
        [Route("api/users/{id:int}/wallet")]
        public HttpResponseMessage AddCurrency(int id)
        {
            throw new Exception("exception");

            HttpContent content = Request.Content;
            var obj = JObject.Parse(content.ReadAsStringAsync().Result);
            int money = int.Parse(obj.SelectToken("addMoney").ToString());

            _repository.Get(id).AddMoney(money);

            var result = new { virtualMoney = money };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [UserAuthorizationFilter]
        [Route("api/users/{id:int}/wallet")]
        public HttpResponseMessage GetCurrency(int id)
        {
            int? currency = _repository.Get(id).Balance ?? 0;

            return Request.CreateResponse(HttpStatusCode.OK, currency);
        }

        [HttpPost]
        [UserAuthorizationFilter]
        [Route("api/users/{id:int}/bets")]
        public HttpResponseMessage PlaceBet(int id, [FromBody]BetRequest bet)
        {
            User user = _repository.Get(id);

            Bet userBet = new Bet
            {
                UserId = bet.UserId,
                BetId = Bet.IdCounter,
                Stake = bet.Stake,
                DiceSum = bet.Sum,
                TimeStamp = DateTime.Now
            };

            userBet.RollDice();

            if (user.BetHistory != null)
            {
                user.BetHistory.Add(userBet);
            }
            else
            {
                user.BetHistory = new List<Bet> { userBet };
            }

            Bet.IdCounter++;

            _repository.Update(user);

            return Request.CreateResponse(HttpStatusCode.OK, userBet);
        }

        [HttpGet]
        [UserAuthorizationFilter]
        [Route("api/users/{id:int}/bets/{betId:int}")]
        public HttpResponseMessage GetBet(int id, int betId)
        {
            Bet bet = _repository.Get(id).GetBet(betId);

            var resultBet = new
            {
                timestamp = bet.TimeStamp,
                stake = bet.Stake,
                win = bet.Win,
                bet = bet.DiceSum,
                actualRoll = bet.ActualRoll
            };

            return Request.CreateResponse(HttpStatusCode.OK, resultBet);
        }

        [HttpGet]
        [UserAuthorizationFilter]
        [Route("api/users/{id:int}/bets")]
        public HttpResponseMessage GetBetsPaginated(int id)
        {
            int? page = null;
            int? size = null;
            string orderBy = null;
            string filter = null;

            User user = _repository.Get(id);
            string[] queryParams = Request.RequestUri.Query.Split('&');

            page = int.Parse(queryParams[0].Split('=')[1]);
            size = int.Parse(queryParams[1].Split('=')[1]);
            orderBy = queryParams[2].Split('=')[1];

            if (queryParams.Length == 4)
            {
                filter = queryParams[3].Split('=')[1];
            }

            List<BetWrapper> userBets = user.GetBetsPaginated(page, size, orderBy, filter);


            return Request.CreateResponse(HttpStatusCode.OK, userBets);
        }

        [HttpDelete]
        [UserAuthorizationFilter]
        [Route("api/users/{id:int}/bets/{betId:int}")]
        public HttpResponseMessage DeleteBet(int id, int betId)
        {
            _repository.Get(id).DeleteBet(betId);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

    }
}
