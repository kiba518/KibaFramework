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

        public List<User> GetComboBoxData()
        { 
            List<User> userList = new List<User>();
            User user1 = new User() { Id = 1, Name = "张三", Age = 11 };
            User user2 = new User() { Id = 2, Name = "李四", Age = 21 };
            User user3 = new User() { Id = 3, Name = "王五", Age = 31 };
            User user4 = new User() { Id = 4, Name = "Kiba518", Age = 101 };
            User user5 = new User() { Id = 5, Name = "张三A", Age = 11 };
            User user6 = new User() { Id = 6, Name = "李四A", Age = 21 };
            User user7 = new User() { Id = 7, Name = "王五A", Age = 31 };
            User user8 = new User() { Id = 8, Name = "Kiba518A", Age = 101 };

            userList.Add(user1);
            userList.Add(user2);
            userList.Add(user3);
            userList.Add(user4);
            userList.Add(user5);
            userList.Add(user6);
            userList.Add(user7);
            userList.Add(user8);

            return userList;
        }

        public void GeDataGridData(User user, int currentPage, int skipNumber, Action<List<User>, int, string> callback)
        {
            #region 准备数据=>等于从数据库读取数据
            List<User> userList = new List<User>();
            User user1 = new User() { Id = 1, Name = "张三", Age = 11 };
            User user2 = new User() { Id = 2, Name = "李四", Age = 21 };
            User user3 = new User() { Id = 3, Name = "王五", Age = 31 };
            User user4 = new User() { Id = 4, Name = "Kiba518", Age = 101 };
            User user5 = new User() { Id = 5, Name = "张三A", Age = 11 };
            User user6 = new User() { Id = 6, Name = "李四A", Age = 21 };
            User user7 = new User() { Id = 7, Name = "王五A", Age = 31 };
            User user8 = new User() { Id = 8, Name = "Kiba518A", Age = 101 };

            userList.Add(user1);
            userList.Add(user2);
            userList.Add(user3);
            userList.Add(user4);
            userList.Add(user5);
            userList.Add(user6);
            userList.Add(user7);
            userList.Add(user8);

            var tempList = from p in userList select p;
            if (user != null)
            {
                if (user.Id > 0)
                {
                    tempList = from p in tempList where p.Id == user.Id select p;
                }
                if (user.Age > 0)
                {
                    tempList = from p in tempList where p.Age == user.Age select p;
                }
                if (!string.IsNullOrWhiteSpace(user.Name))
                {
                    tempList = from p in tempList where p.Name == user.Name select p;
                }
            }
            #endregion

            List<User> resultList = tempList.OrderByDescending(p => p.Id).Skip((currentPage - 1) * skipNumber).Take(skipNumber).ToList();
            int count = userList.Count;
            callback(resultList, count, "查询成功");
        }
    }
}
