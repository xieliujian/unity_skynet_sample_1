local skynet = require "skynet"
local netpack = require "skynet.netpack"
local socket = require "skynet.socket"

local client_fd = ...
client_fd = tonumber(client_fd)

skynet.register_protocol {
    name = "client",
    id = skynet.PTYPE_CLIENT,

    --需要将网路数据转换成lua字符串，不需要打包，所以不用注册pack函数
    unpack = skynet.tostring,
}

local function task(msg)
    print("recv from fd", client_fd, msg)

    local reqmsgid = string.unpack("<L", msg);
    local msgoffset = 8;
    local reqstr = string.sub(msg, msgoffset + 1);

    print("" .. reqmsgid);
    print(reqstr);

    -- rspmsg
    local rspstr = "Say(说) : " .. reqstr;
    local rspmsgid = 10001;

    local pack = string.pack("<L", rspmsgid);
    pack = pack .. rspstr;

    local package = string.pack("<s2", pack)

    --响应消息的时候直接通过fd发送出去
    socket.write(client_fd, package)
end

skynet.start(function()
    --注册client消息专门用来接收网络数据
    skynet.dispatch("client", function(_, _, msg)
        task(msg)
    end)

    skynet.dispatch("lua", function(_, _, cmd)
        --注册lua消息，来退出服务
        if cmd == "quit" then
            skynet.error(fd, "agent quit")
            skynet.exit()
        end
    end)
end)
