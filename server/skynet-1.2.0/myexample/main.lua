---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by xieliujian.
--- DateTime: 2019/8/11 22:31
---
---
local skynet = require "skynet"

-- 启动服务(启动函数)
skynet.start(function()

    -- 启动函数里调用Skynet API开发各种服务
    print("======Server start=======")

    local gateserver = skynet.newservice("mygateserver") --启动刚才写的网关服务
    skynet.call(gateserver, "lua", "open", {--需要给网关服务发送open消息，来启动监听
        port = 8888, --监听的端口
        maxclient = 64, --客户端最大连接数
        nodelay = true, --是否延迟TCP
    })
    
    skynet.exit()
end)
