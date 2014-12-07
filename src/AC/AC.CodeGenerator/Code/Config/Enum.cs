using System.ComponentModel;

namespace AC.Code.Config
{
    /// <summary>
    /// Dao实现方式
    /// DbHelper:使用自我封装的DbHelper形式
    /// </summary>
    public enum DaoStyle
    {
        [Description("使用DZ.Dao类库中的DbHelper类库，非常强大，建议使用!")]
        DbHelper=1,
        [Description("使用微软的SqlHelper底层封装类，凑合着用吧...")]
        SqlHelper=2
    }

    /// <summary>
    /// 方法调用方式
    /// </summary>
    public enum CallStyle
    {
        /// <summary>
        /// 直接New 如：UserDAL userDal = new UserDAL();
        /// </summary>
        [Description("使用传统创建对象方式new一个对象:IUserService userService = new UserServiceImpl();")]
        NewObj=1,

        /// <summary>
        /// 使用Spring.Net解耦
        /// </summary>
        [Description("使用Spring.Net实现依赖注入：IUserService userService = ServiceLocator.GetService(IUserService)")]
        SpringNew=2,
    }

    /// <summary>
    /// 代码分层
    /// </summary>
    public enum CodeLayer
    {
        /// <summary>
        /// 普通三层架构 BLL(Service层)直接调用DAL(Dao)，对象类用Modal(DTO)
        /// BLL->DAL
        /// BLL类名称：UserBLL(表名+BLL)
        /// DAL类名称：UserDAL(表名+DAL)
        /// Modal层类名：UserInfo(表名+Info)
        /// </summary>
        [Description("普通三层架构 BLL直接调用DAL，对象类用Modal:UserBLL->UserDAL use UserInfo")]
        ThreeLayer=1,

        /// <summary>
        /// Service三层架构 Service -> Dao with DTO
        /// Service类名称：UserService(表名+Service)
        /// Dao类名称：UserDao(表名+Dao)
        /// DTO层类名：UserDTO(表名+DTO)
        /// </summary>
        [Description("Service三层架构:UserService->UserDao use UserDTO")]
        ServiceThreeLayer=2,

        /// <summary>
        /// Service标准五层架构 Service ServiceImpl Domain Dao DTO
        /// Service: IUserService
        /// ServiceImpl: UserService
        /// Domain: IUserRepos
        /// Dao:UserDao
        /// DTO: UserDTO
        /// </summary>
        [Description("Service标准五层架构 Service ServiceImpl Domain Dao DTO")]
        ServiceLayerWithDomain=3,

        /// <summary>
        /// Service标准五层结构去掉Domain层接口 Service ServiceImpl Dao DTO
        /// </summary>
        [Description("Service标准五层结构去掉Domain层接口 Service ServiceImpl Dao DTO")]
        ServiceLayerWithoutDomain=4,
    }
}