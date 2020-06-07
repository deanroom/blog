# 云原生实践1-Asp.NET(!Asp.NET Core) 容器化实践

目前基于云原生理念构建应用已是主流,然而有些同仁手上可能还有在维护的项目或者产品使用的是基于 .NET Framework 的 Asp.NET 而非 .NET Core。

那么为了迎接云原生是否需要全部把现有程序重写为Java或者Golang呢，对于资源充足的团队或者公司或为可行，但是对于人少业务多的团队而言，今天给大家提供另外一种选项，
使已经跑了五六年甚至十多年左右的代码库也能搭上云原生的顺风车。

- Asp.NET回顾
- Mono简介
- Docker简介
- 云原生简介
- 容器化实践
- .NET 展望


## Asp.NET 回顾
    
2007年入坑.NET ，始于.NET 2.0，一直到2014年前后慢慢开始接触Asp.NET Core的前身ASP.NET vNext，后逐渐少去折腾.NET FrameWork了。单论感性，我会说.NET是款很舒服的工具集，要什么有什么，尤其是针对企业开发的时候。最终的结论是：不论是个人-团队的技术成长甚或企业的技术风险-债务规避，真正跨平台语言和工具带来地感受和价值远比用起来舒服来得重要。客观地说 .NET Framework以及Asp.NET都极其优秀，至少从诞生到2015年前后到市场占有可见其功。可是隐患也颇深，跟一个同事聊天的时候，得出一个结论，微软的路子就是你累就给凳子，困了就给床，什么都想做好都结局只有什么都很难做到极致。如果能更早些走开源社区路子，现在国内也不会存在大量公司/.NET程序员转技术栈。很多欧洲公司也都在去微软化，这可能还包括其他很多因素但是一个不能真正跨平台单工具这现今云原生为主流的情形下，实在是很难受。

## Mono 简介

Mono与.NET相比则是走了另外一条道路，完全社区化，其官网定义-Sponsored by Microsoft, Mono is an open source implementation of Microsoft's .NET Framework based on the ECMA standards for C# and the Common Language Runtime。简单说就是.NET Framework的开源实现。目前最新版本支持到.NET 4.7，不支持WPF,WWF，对于Asp.NET是基本全部支持，仅有少量异步栈受限。运行平台则是macOS,Linux,Windows一个都不能少。

## Docker简介

WiKi定义：Docker利用Linux核心中的资源分离机制，例如cgroups，以及Linux核心名字空间（namespaces），来创建独立的容器（containers）。这可以在单一Linux实体下运作，避免引导一个虚拟机造成的额外负担。
    通俗地讲，Docker就是进程级别的虚拟机，在进程级隔离资源并虚拟一个完整地CPU，网络，内存，磁盘等基础设施，对于应用程序来讲自己是独占一个完备的操作系统。因为是进程级别所以其创建销毁的效率非常高，一秒以内无痛重启。Docker是容器技术的一种典型实现，容器技术又是虚拟化技术的延续。


## 云原生简介    

CNCF以及云原生(Cloud Native)定义：
    
    云原生技术有利于各组织在公有云、私有云和混合云等新型动态环境中，构建和运行可弹性扩展的应用。云原生的代表技术包括容器、服务网格、微服务、不可变基础设施和声明式API。

    这些技术能够构建容错性好、易于管理和便于观察的松耦合系统。结合可靠的自动化手段，云原生技术使工程师能够轻松地对系统作出频繁和可预测的重大变更。

    云原生计算基金会（CNCF）致力于培育和维护一个厂商中立的开源生态系统，来推广云原生技术。我们通过将最前沿的模式民主化，让这些创新为大众所用。    

 ## 容器化实践   
    
云原生牵涉范围很广，内容，工具，理念也很多。但是第一步定是容器化，否则我们的应用就很难使用基于云原生的工具和理念，所以接下来我就带大家体验下如何把现有的Asp.NET 应用程序进行容器化.

1. 跟Windows平台部署Asp.NET程序一样，我们需要一个类似IIS的托管工具，Mono提供三种实现：
    
   1. Apache hosting: use mod_mono, a module that allows Apache to serve ASP.NET applications.
    
   2.  FastCGI hosting: use the FastCGI hosting if you have a web server that supports the FastCGI protocol (for example Nginx) for extending the server. You also may use a web server that only has support for CGI using 
    cgi-fcgi.
   3. XSP: this is a simple way to get started, a lightweight and simple webserver written in C#.
   xsp的化是一种轻量级实现，如果有同学尝试过Visual Studio For Mac,或者Rider你会发现里面运行的就是XSP让你去调试基于Asp.NET Framework的应用程序。
   
   这里我采用的是FastCGI+Nginx的方式，参考github上[一位小哥的Dockerfile](!https://github.com/junalmeida/docker-mono-web)
2. 实践
   1. Dockerfile，定义容器如何编译，除了安装必要组件，其中还有nginx和[supervisord](!http://supervisord.org/)的自定义配置
```Dockerfile
   
FROM mono:latest

LABEL maintainer="Marcos Junior <junalmeida@gmail.com>"

RUN apt-get update \
  && apt-get install -y \
      iproute2 supervisor ca-certificates-mono fsharp mono-vbnc nuget \
      referenceassemblies-pcl mono-fastcgi-server4 nginx nginx-extras \
  && rm -rf /var/lib/apt/lists/* /tmp/* \
  && echo "daemon off;" | cat - /etc/nginx/nginx.conf > temp && mv temp /etc/nginx/nginx.conf \
  && sed -i -e 's/www-data/root/g' /etc/nginx/nginx.conf

COPY nginx/ /etc/nginx/
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
 
EXPOSE 80

ENTRYPOINT [ "/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf" ]
```

这个容器我已经编译过，实际项目直接再其上再次封装既可，一层层加上去，十分有趣。比如在实际项目中的Dockerfile就类似

```
FROM [定制编译后地镜像存放地repository地址]:latest as build
WORKDIR /src
COPY #CODE PATH#
RUN nuget restore XXXX/XXXX.sln
RUN msbuild XXXX/XXXX/XXXX.csproj

FROM [定制编译后地镜像存放地repository地址]:latest

# Change timezone to local time
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo "Asia/Shanghai" >/etc/timezone
RUN dpkg-reconfigure -f noninteractive tzdata
 
COPY --from=build /src/XXXX/XXXX/bin/ /var/www/app/bin
COPY XXXX/XXXX/build/nginx/ /etc/nginx/
COPY XXXX/XXXX/build/pools/ /etc/mono/pools/
COPY XXXX/XXXX/build/app/ /var/www/app/
COPY XXXX/XXXX/build/supervisord.conf /etc/supervisor/conf.d/supervisord.conf
 
EXPOSE 80

ENTRYPOINT [ "/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf" ]


```

   2. Nginx配置
```
server {
         listen   80;
         access_log   /var/log/nginx/mono-fastcgi.log;
         root /var/www/;
         server_tokens off;
         more_clear_headers Server X-AspNet-Version;

         location / {
                 index index.html index.htm default.aspx Default.aspx;
                 fastcgi_index #程序默认路径#;
                 fastcgi_pass unix:/var/run/mono-fastcgi.sock;
                 include /etc/nginx/fastcgi_params;
         }
 }
```
   3.  打包完上传到阿里云容器镜像服务到，私有镜像仓库
   4.  编写一个docker-compose
```docker-compose.yaml
version: '3.4'

services:
   dreamwork.legacywebapi:
     image: registry.cn-hangzhou.aliyuncs.com/xxxx/xxxx:latest
     ports:
       - "80"

```
   docker compose 可以存放在git服务器上，这样每次有改动都有版本控制，然后服务器上只要安装的有docker-compose,执行 docker-compose up -d 直接部署。
 
    总的来说就是完成：代码使用容器打包编译并上传到自己到容器仓库，然后服务器从容器仓库直接获取容器进行部署，这只是devops中到一小步，但是却是整体基于云原生去进行程序开发部署的关键一步。尤其对于背负很多技术债务的项目，再全部使用新的语言，框架重写之外也是有第二种选择的。
## .NET Core 与 .NET 5
.NET Core已经经历四年多了，到今年.NET Core 3.1的发布也算是一个里程碑 **The next release after .NET Core 3.1 will be .NET 5. The .NET Framework will be deprecated, and .NET 5 will be the only .NET going forward**，生态需要逐步完善，很可以期待。更振奋人心的是微软宣布的.NET 5的路线图的发布

| Milestone                 | Release Date |
|---------------------------|--------------|
| .NET Core 3.0.x (servicing) | Maintenance. Approximately every 1-2 months or as needed. |
| .NET Core 3.1.x (servicing) | LTS (Long Term Support) release. Approximately every 1-2 months or as needed. |
| .NET 5.0 | Release scheduled for November 2020 |
| .NET 6.0 | LTS (Long Term Support) release, scheduled for November 2021 |
| .NET 7.0 | Release scheduled for November 2022 |
| .NET 8.0 | LTS (Long Term Support) release, scheduled for November 2023 |

不论在云原生时代你倾向使用什么语言和工具，至少.NET Core都是值得你去了解的。