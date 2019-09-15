#愿景
搭建一套基于云原生的微服务架构平台，能满足公司现有和将来的业务支撑，横向和纵向解耦业务实现。能够持续集成公司新业务，能够保证现有业务的稳定迭代。

![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558511024_2.png)
1. ###用户
	定义为所有使用本平台的学员，会员，员工等所有参与者。
2. ###设备
PC,手机，平板以及物联网设备。
3. ###App服务
	针对设备提供各种终端接入支持，主要目的为适配各种终端。
4. ###Api服务
提供平台对外服务规范和约束，为App服务提供支持，为公开服务提供支持。
5. ###基础微服务
- ####业务微服务
实现业务功能比如订单，产品，试听，题库等业务实现
- ####通用微服务
实现登录，支付，认证，鉴权等
- ####基础设施微服务
实现基础设施功能，比如缓存，定时任务，监控，健康检查，日志等

#技术栈
##后端
###NET Core,Java,Python,Go等
基于NET Core 实现，必要的时候引入Python，Java，GO等实现相关功能，在不同的时间节点使用最合适的资源和工具构建服务。
使用Docker构建服务镜像，并使用Kubernetes编排支撑生产环境。开发、测试，验证环境可使用Docker-Compose编排维护。
###数据服务
使用Mysql构建业务模型，Redis、Mongodb等作为高并发高可用支撑，尽可能采用阿里云RDS方案，必要时引入其它分布式RDMS方案，对于题库考试等可斟酌在必要时候是否采用Cassandra支撑多写操作。同时使用ElasticSearch构建日志，全文检索环境。
##小程序
目前继续使用微信原生实现，同时探索其它通用框架。
##APP
目前继续使用Android，IOS原生开发，同时探索混合框架的可行性。
##Web
牵涉Seo部分使用NetCore MVC构建，并实现网站静态化，
非Seo的前端和后端管理采用Vue构建。
##直播录播平台
尝试体验其它直播、录播平台、甚至在必要时候自建直播录播服务。
##运行时
###Linux
采用Centos或者Ubuntu等作为基础操作系统 
###Docker
所有后端交付服务均使用docker构建
###Docker Compose
开发、测试、验证环境使用DockerCompose快速编排部署，快速验证。
###Kubernetes
生产环境，使用kubernetes集群编排容器服务实现负载均衡，水平扩容，健康检查等。
###云平台其它能力接入
必要时候引入平台提供的负载均衡，CDN加速等功能。
#开发规范
##代码结构
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558664385_50.png)

##代码仓库：
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558332692_43.png)
 代码仓库使用guoba-develop，分为三个组：
- backend 
	-  微服务实现
	-  发布脚本维护
- frontend 
	- 小程序
	- H5页面
	- 门户网站
	- ...
- app
	- 安卓App
	- IOSApp

各组按照相应规范组织自己的代码仓库，命名空间推荐：dreamwork.[平台名称].[应用名称].[服务名称].[API/APP/WAPP/SVC等]
可以根据需要创建多代码仓库，产品终端
版本号推荐0.1.0开始
各组建立自己固定的代码规范。
用户组GUOBA-WEB作为留档备份，维护现有程序。
##后端开发规范
###单微服务架构
一个外部依赖Dreamwork.core、完全依赖注入、Controller标准化Webapi、借助DDD定义业务模型、Service聚合业务逻辑、Infrastructure定义仓储实现、读写分离保障高并发读性能、Dockerfile定义镜像构建方式

1. 单个微服务依赖目前只有一个，使用Dreamwork.Core提供各种基础组件实现,后续会提交到Nuget维护，提供一个标准微服务应该具有的基础组件和标准接口，方便单个微服务快速且标准地建立，并专注于业务实现，降低各种基础设施使用门槛并可无缝迁移到不同三方组件。
- RESTful-API基础定义
定义RESTful-API的基础实现，方便微服务中的Controller快速实现
- 缓存组件
封装分布式缓存，降低缓存维护成本，并可对缓存方便进行集中管理。
- 消息队列组件
	提供基础消息定义降低微服务中消息队列使用门槛，便于整体随时实现迁移，方便消息队列管理。
- 数据持久化
实现RDMS，Nosql的驱动和定义，并提供统一接口，在业务微服务不用考虑具体的仓储底层实现，所有业务微服务在业务实现中可以使用同一仓储接口而无需关心具体是哪种。实现无缝业务数据迁移不同仓储平台。
- RPC调用基础实现
隔离具体RPC实现方案降低微服务中RCP使用难度。
- ...
2. 依据[ASP.NET Core Web API apps](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.2)创建单个微服务。
	- ####Startup.cs
	- ConfigureServices
	![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558690484_59.png)
	已经实现MVC基本配置
	以上配置默认包括跨域支持、Token认证、Swagger等基础组  件初始化。
	- Configure		
		![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558690555_72.png)
 		以上配置启用默认服务
	- ####Controllers
	- API Base Controllers 
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558690725_73.png)
	- Normal API Controllers
	``` 
	[ServiceFilter(typeof(ValidateAsyncFilter))]
	    public class BroadcastController : MiniBuildControllerBase
	    {
	        private readonly IOrderService _orderservice;
	
	        private readonly IBroadcastService _broadcastService;
	        public BroadcastController(IOrderService orderservice,
	            IBroadcastService broadcastService
	            )
	        {
	            _orderservice = orderservice;
	            _broadcastService = broadcastService;
	        }
	        /// <summary>
	        /// 删除直播
	        /// </summary>
	        /// <param name="id"></param>
	        /// <returns></returns>
	        [HttpDelete("{id}")]
	        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
	        public async Task<IActionResult> DeleteAsync(Guid id)
	        {
	            var result = await _broadcastService.DeleteAsync(new Guid(CurrentUserId), id);
	            return result.IsSuccess ? Success(result.Data, "删除成功") : Error<bool>(result.Message);
	        }
	       
	        /// <summary>
	        /// 获取直播分页数据
	        /// </summary>
	        /// <param name="model"></param>
	        /// <returns></returns>
	        [HttpPost]
	        [ProducesResponseType(typeof(PagedJsonResult<BroadcastListResult>), (int)HttpStatusCode.OK)]
	        public async Task<IActionResult> GetPageListAsync(QueryPageInfo model)
	        {
	            var result = await _broadcastService.GetListAsync();
	            if (result.IsSuccess)
	            {
	                var totalCount = result.Data.Count();
	                return Success(
	                     result.Data.PageItems(model),
	                     new PageResultInfo
	                     {
	                         PageIndex = model.PageIndex,
	                         PageSize = model.PageSize,
	                         TotalCount = totalCount,
	                         TotalPage = model.PageSize == 0?0:(long)Math.Ceiling(totalCount / (model.PageSize * 1.0))
	                     });
	            }
	            return Error(result.Message);
	        }
	
	        [HttpPost]
	        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
	        [ProducesResponseType((int)HttpStatusCode.NotFound)]
	        public async Task<IActionResult> UpdateSortAsync(UpdateSortReq model)
	        {
	            var result = await _broadcastService.UpdateSortAsync(model);
	            return result.IsSuccess ? Success(result.Data, "操作成功") : Error<bool>(result.Message);
	        }
	``` 
	3. Domain、Entity定义业务模型，仓储接口IRepository
	这里借用DDD中领域的一些概念但是更轻量化地定义业务模型和仓储，主要实现静态业务模型和仓储的定义，同时可支持一定的状态转换逻辑。此处可以完整抽象业务逻辑也可放任到Service中实现，一切标准建立在团队现实能力之上。
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558605838_15.png)
	4. Services根据Domain和Irepository实现业务逻辑封装
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558605891_47.png)
	5. Infrastructure实现持久化具体实现，此处牵涉表详细定义，仓储平台具体实现。这里可以制定业务数据是存储在RDMS还是Nosql或者是网络存储等，主要承担解耦业务数据定义和真实仓储实现。
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558605792_56.png)
Repository使用EFCore封装数据仓储实现，标准化业务数据存储。
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558689359_91.png)
	6. 仓储实现读写分离，可以访问从数据库,
可以使用专门定义为查询的DbContext借助EFCore，也可以使直接使用Sql然后使用Dapper构建
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558689519_18.png)
	7. Dockerfile
定义单个微服务docker镜像构建脚本，提供微服务标准的交付物。	
![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1558690794_41.png)
3. 其它规范：
	- 列表
	排序编号->修改时间->创建时间 倒序排序
###微服务通讯
1. RESTful API
    1. 使用Controller定义接口信息包括:
       1. 统一的API基类，定义当前用户，返回Json处理，跨域策略，路由模版等。
       2. 每个Controller必要的时候添加特有的自定义标签等，对于外部资源使用依赖注入引入
       ```private readonly VhallApiClient _vhallClient;
        private readonly IBroadcastService _broadcastService;
        public BroadcastController(VhallApiClient vhallClient,
            IBroadcastService broadcastService
            )
        {
            _vhallClient = vhallClient;
            _broadcastService = broadcastService;
        } 
        ```
        3. 每个Action必须定义HttpMethod、返回数据类型、路由、xml注释等信息，有明确的逻辑成功或者失败描述。
        每个Action约定只要连通均返回状态200，进一步信息通过json描述。 
        ``` [HttpGet("{id}")]
        [ProducesResponseType(typeof(BroadcastDetailResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetailAsync(Guid id)
        {
            var result = await _broadcastService.GetDetailAsync(id);
            return result.IsSuccess ? Success(result.Data) : Error<BroadcastDetailResult>(result.Message);
        }
        ```
       4. Swashbuckle会根据上面的接口定义生成接口文档。
2. 消息队列
		使用[DotNetCore.CAP](https://github.com/dotnetcore/CAP)进行消息队列组件的封装。
		CAP 是一个基于 .NET Standard 的 C# 库，它是一种处理分布式事务的解决方案，同样具有 EventBus 的功能，它具有轻量级、易使用、高性能等特点。
		预览（OverView）
		在我们构建 SOA 或者 微服务系统的过程中，我们通常需要使用事件来对各个服务进行集成，在这过程中简单的使用消息队列并不能保证数据的最终一致性， CAP 采用的是和当前数据库集成的本地消息表的方案来解决在分布式系统互相调用的各个环节可能出现的异常，它能够保证任何情况下事件消息都是不会丢失的。
		你同样可以把 CAP 当做 EventBus 来使用，CAP提供了一种更加简单的方式来实现事件消息的发布和订阅，在订阅以及发布的过程中，你不需要继承或实现任何接口。	
		![图片描述](/tfl/captures/2019-05/tapd_43121315_base64_1559089231_26.png)
3. RPC
        - grpc
		- thrift
##小程序开发规范
##APP开发规范
#微服务划分
##第一阶段
###内部服务
   1. 后台管理
      - 企业微信集成登录
      - 聚合各个微服务的配置，业务管理
   2. 订单
      - 支撑订单业务，提供完善的订单管理，统计分析 
   3. 产品服务
	  - 产品业务实现
   4. 支付
      - 支付内部接口		
###公开服务
	API网关
         1. ####登录认证
			实现基于[IdentityServer4](https://github.com/IdentityServer/IdentityServer4)的OpenID Connect和OAuth 2.0认证，保障用户，API资源的完整认证流程。
			封装微信登录。
         2. ####支付
            - 支付平台回调
         3. ####App API
         4. ####小程序 API
##第二阶段
2. ####登录认证
2. ####支付
2. ####订单
2. ####课程
2. ####题库
2. ####学习中心
2. ####数据分析
2. ####App
2. ####小程序
2. ####服务注册发现健康检查
##第三阶段
3. ####用户认证
3. ####终端认证
3. ####支付
3. ####订单
3. ####课程
3. ####题库
3. ####学习中心
3. ####大数据中心
3. ####BI智能分析
3. ####AI服务
3. ####App多端服务
3. ####小程序多端服务
3. ####服务注册发现健康检查
##第四阶段
4. ####业务
4. ####大数据
4. ####AI
4. ####多终端
4. ####物联网探索
4. ####自建云服务平台
##第...阶段
###完备知识平台
