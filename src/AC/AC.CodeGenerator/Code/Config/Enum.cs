using System.ComponentModel;

namespace AC.Code.Config
{
    public enum CodeType
    {
        CSharp=1,
        Java=2,
    }
    /// <summary>
    /// Daoʵ�ַ�ʽ
    /// DbHelper:ʹ�����ҷ�װ��DbHelper��ʽ
    /// </summary>
    public enum DaoStyle
    {
        [Description("ʹ��DZ.Dao����е�DbHelper��⣬�ǳ�ǿ�󣬽���ʹ��!")]
        DbHelper=1,
        [Description("ʹ��΢���SqlHelper�ײ��װ�࣬�պ����ð�...")]
        SqlHelper=2
    }

    /// <summary>
    /// �������÷�ʽ
    /// </summary>
    public enum CallStyle
    {
        /// <summary>
        /// ֱ��New �磺UserDAL userDal = new UserDAL();
        /// </summary>
        [Description("ʹ�ô�ͳ��������ʽnewһ������:IUserService userService = new UserServiceImpl();")]
        NewObj=1,

        /// <summary>
        /// ʹ��Spring.Net����
        /// </summary>
        [Description("ʹ��Spring.Netʵ������ע�룺IUserService userService = ServiceLocator.GetService(IUserService)")]
        SpringNew=2,
    }

    /// <summary>
    /// ����ֲ�
    /// </summary>
    public enum CodeLayer
    {
        /// <summary>
        /// ��ͨ����ܹ� BLL(Service��)ֱ�ӵ���DAL(Dao)����������Modal(DTO)
        /// BLL->DAL
        /// BLL�����ƣ�UserBLL(����+BLL)
        /// DAL�����ƣ�UserDAL(����+DAL)
        /// Modal��������UserInfo(����+Info)
        /// </summary>
        [Description("��ͨ����ܹ� BLLֱ�ӵ���DAL����������Modal:UserBLL->UserDAL use UserInfo")]
        ThreeLayer=1,

        /// <summary>
        /// Service����ܹ� Service -> Dao with DTO
        /// Service�����ƣ�UserService(����+Service)
        /// Dao�����ƣ�UserDao(����+Dao)
        /// DTO��������UserDTO(����+DTO)
        /// </summary>
        [Description("Service����ܹ�:UserService->UserDao use UserDTO")]
        ServiceThreeLayer=2,

        /// <summary>
        /// Service��׼���ܹ� Service ServiceImpl Domain Dao DTO
        /// Service: IUserService
        /// ServiceImpl: UserService
        /// Domain: IUserRepos
        /// Dao:UserDao
        /// DTO: UserDTO
        /// </summary>
        [Description("Service��׼���ܹ� Service ServiceImpl Domain Dao DTO")]
        ServiceLayerWithDomain=3,

        /// <summary>
        /// Service��׼���ṹȥ��Domain��ӿ� Service ServiceImpl Dao DTO
        /// </summary>
        [Description("Service��׼���ṹȥ��Domain��ӿ� Service ServiceImpl Dao DTO")]
        ServiceLayerWithoutDomain=4,
    }

    /// <summary>
    /// ��������
    /// </summary>
    public enum NameStyle
    {
        [Description("UpperCamelCase")]
        UpperCamelCase = 1,
        [Description("lowerCamelCase")]
        lowerCamelCase = 2,
        [Description("ALL_UPPER")]
        ALL_UPPER = 3,
        [Description("all_lower")]
        all_lower = 4,
        [Description("First_upper")]
        First_upper = 5,
    }

    public enum CodeLanguage
    {
        [Description("C#")]
        CSharp = 1,
        [Description("Java")]
        Java = 2,
    }

    public enum EntityKind
    {
        [Description("���ͼ������ռ�")]
        TypesAndNamespace = 1,
        [Description("�ӿ�")]
        Interface = 2,
        [Description("ö�ٳ�Ա")]
        EnumMembers=3,
    }
}