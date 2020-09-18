-- table: public.app_audit_logs

-- drop table public.app_audit_logs;

create table public.app_audit_logs
(
    id bigint not null default snow_flake_id(),
    request_path character varying (256) collate pg_catalog."default" not null,
    request_method character varying(8) collate pg_catalog."default" not null,
    user_name character varying(64) collate pg_catalog."default",
    start_at timestamp without time zone not null,
    duration double precision not null,
    response_code integer not null,
    controller_name character varying(64) collate pg_catalog."default",
    action_name character varying(64) collate pg_catalog."default",
    description character varying(256) collate pg_catalog."default",
    ip character varying(64) collate pg_catalog."default",
    constraint pk_app_audit_logs primary key (start_at, user_name, request_path, request_method)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_audit_logs
    owner to postgres;
comment on table public.app_audit_logs
    is '审计日志';

comment on column public.app_audit_logs.id
    is '审计日志id';

comment on column public.app_audit_logs.request_path
    is '请求路径';

comment on column public.app_audit_logs.request_method
    is '请求方法';

comment on column public.app_audit_logs.user_name
    is '用户名';

comment on column public.app_audit_logs.start_at
    is '开始时间';

comment on column public.app_audit_logs.duration
    is '耗时(毫秒)';

comment on column public.app_audit_logs.response_code
    is '响应状态码';

comment on column public.app_audit_logs.controller_name
    is '控制器名称';

comment on column public.app_audit_logs.action_name
    is '动作名称';

comment on column public.app_audit_logs.description
    is '描述';
comment on column public.app_audit_logs.ip
    is '客户端 IP 地址';

alter table public.app_audit_logs
    add column host_name character varying(32);

comment on column public.app_audit_logs.host_name
    is '请求的主机名';

-- TimescaleDB hyper table def;

-- select create_hypertable('public.app_audit_logs', 'start_at');
-- select add_dimension('public.app_audit_logs', 'user_name', number_partitions => 4);
-- select add_dimension('public.app_audit_logs', 'request_path', number_partitions => 4);
-- select add_dimension('public.app_audit_logs', 'request_method', number_partitions => 4);
