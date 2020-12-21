-- table: public.connections

-- drop table public.connections;

create table public.connections
(
    id bigint not null default snow_flake_id(),
    name character varying(64) collate pg_catalog."default" not null,
    database_type character varying(16) collate pg_catalog."default" not null,
    server_address character varying(64) collate pg_catalog."default" not null,
    server_port integer,
    database_name character varying(64) collate pg_catalog."default" not null,
    username character varying(64) collate pg_catalog."default",
    password character varying(256) collate pg_catalog."default",
    timeout integer,
    is_deleted boolean not null default false,
    constraint pk_connections primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.connections
    owner to postgres;

comment on table public.connections
    is '数据库连接';

comment on column public.connections.id
    is '连接id';

comment on column public.connections.name
    is '连接名称';

comment on column public.connections.database_type
    is '数据库类型（postgres、mssql、mysql、oracle、sqlite等）';

comment on column public.connections.server_address
    is '服务器地址';

comment on column public.connections.server_port
    is '服务器端口';

comment on column public.connections.database_name
    is '数据库名称';

comment on column public.connections.username
    is '数据库用户名';

comment on column public.connections.password
    is '数据库密码';

comment on column public.connections.timeout
    is '超时时间';

comment on column public.connections.is_deleted
    is '是否已删除（软删除）';
