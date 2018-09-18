using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class TestDataProxy
    {
        public List<User> GetListDataProxy()
        {
            List<User> userList = new List<User>();
            User user1 = new User() { Id = 1, Name = "张三", Age = 11 };
            User user2 = new User() { Id = 2, Name = "李四", Age = 21 };
            User user3 = new User() { Id = 3, Name = "王五", Age = 31 };
            User user4 = new User() { Id = 4, Name = "Kiba518", Age = 101 };

            userList.Add(user1);
            userList.Add(user2);
            userList.Add(user3);
            userList.Add(user4);
            return userList;
        }
    }
}
