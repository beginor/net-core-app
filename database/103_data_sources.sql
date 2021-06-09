-- table: public.data_sources

-- drop table public.data_sources;

create table public.data_sources
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
    constraint pk_data_sources primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.data_sources
    owner to postgres;

comment on table public.data_sources
    is '数据源';

comment on column public.data_sources.id
    is '数据源id';

comment on column public.data_sources.name
    is '数据源名称';

comment on column public.data_sources.database_type
    is '数据库类型（postgres、mssql、mysql、oracle、sqlite等）';

comment on column public.data_sources.server_address
    is '服务器地址';

comment on column public.data_sources.server_port
    is '服务器端口';

comment on column public.data_sources.database_name
    is '数据库名称';

comment on column public.data_sources.username
    is '数据库用户名';

comment on column public.data_sources.password
    is '数据库密码';

comment on column public.data_sources.timeout
    is '超时时间';

comment on column public.data_sources.is_deleted
    is '是否删除';
