using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Classes;
using WebAPI.Exceptions;

namespace WebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int? Balance { get; set; }

        public string AuthToken { get; set; }

        public List<Bet> BetHistory { get; set; }

        #region Public methods

        public void ChangePassword(string oldPass, string newPass)
        {
            if (this.Password == oldPass)
            {
                this.Password = newPass;
            }
            else
            {
                throw new Exception("Access denied");
            }
        }

        public void CreateBet(Bet bet)
        {
            this.BetHistory.Add(bet);

            int balanceChange = bet.Win - bet.Stake;

            this.Balance += balanceChange;
        }

        public Bet GetBet(int betId)
        {
            Bet bet = this.BetHistory.FirstOrDefault(t => t.BetId == betId);

            if (bet == null)
                throw new BadRequestException("ID does not exist");

            return bet;
        }

        public void DeleteBet(int betId)
        {
            Bet bet = this.BetHistory.FirstOrDefault(b => b.BetId == betId);

            if (bet == null)
                throw new BadRequestException("Wrong betId");

            TimeSpan timeSinceLastThrow = DateTime.Now - bet.TimeStamp;

            if (timeSinceLastThrow.Minutes <= 1)
                RemoveBetEffects(bet);
            else
                throw new ForbiddenException("The last throw was more than a minute ago");
        }

        public void UpdateUser(UserUpdateRequest editedUser)
        {
            if (editedUser.OldPassword != null)
            {
                if (editedUser.NewPassword != null)
                    UpdatePassword(editedUser.OldPassword, editedUser.NewPassword);
                else
                    throw new BadRequestException("Invalid values");
            }
            else
            {
                this.FullName = editedUser.FullName ?? this.FullName;
                this.Email = editedUser.Email ?? this.Email;
            }
        }

        public List<BetWrapper> GetBetsPaginated(int? page, int? size, string orderBy, string filter)
        {
            bool invalidOrderByParams = !orderBy.CompareCaseIgnorant("timestamp") &&
                                        !orderBy.CompareCaseIgnorant("win");

            bool invalidFilterParams = filter != null && !filter.CompareCaseIgnorant("win") &&
                                       !filter.CompareCaseIgnorant("lose");

            bool invalidParams = invalidOrderByParams || invalidFilterParams || !page.HasValue || !size.HasValue;

            if (invalidParams)
            {
                throw new Exceptions.UnauthorizedException("Invalid parameters");
            }

            List<BetWrapper> userBets = this.BetHistory.Select(bet => new BetWrapper(bet)).
                           Skip(page.Value * size.Value).Take(size.Value).ToList();

            userBets = orderBy.CompareCaseIgnorant("timestamp")
                ? userBets.OrderBy(bet => bet.TimeStamp).ToList()
                : userBets.OrderBy(bet => bet.Win).ToList();

            if (filter != null)
            {
                userBets = filter.CompareCaseIgnorant("win")
                    ? userBets.Where(bet => bet.Win != 0).ToList()
                    : userBets.Where(bet => bet.Win == 0).ToList();
            }

            return userBets;
        }
        #endregion

        #region Private methods
        private void RemoveBetEffects(Bet bet)
        {
            this.BetHistory.RemoveAll(t => t.BetId == bet.BetId);
            int balanceChange = bet.Win - bet.Stake;

            this.Balance -= balanceChange;
        }

        private void UpdatePassword(string oldPassword, string newPassword)
        {
            if (this.Password == oldPassword)
                this.Password = newPassword;
            else
                throw new BadRequestException("Invalid credentials.");
        }
        #endregion

        public void AddMoney(int money)
        {
            if (money < 0)
            {
                throw new BadRequestException("Invalid data.");
            }

            this.Balance = this.Balance.HasValue ? this.Balance += money : money;
        }
    }
}