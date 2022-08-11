-- table: public.app_logs

-- drop table if exists public.app_logs;

create table if not exists public.app_logs
(
    id bigint not null default snow_flake_id(),
    created_at timestamp without time zone not null,
    thread character varying(8) collate pg_catalog."default" not null,
    level character varying(16) collate pg_catalog."default" not null,
    logger character varying(256) collate pg_catalog."default",
    message character varying(4096) collate pg_catalog."default",
    exception character varying(4096) collate pg_catalog."default",
    constraint pk_app_logs primary key (id)
)

tablespace pg_default;

alter table if exists public.app_logs
    owner to postgres;

comment on table public.app_logs
    is '应用程序日志';

comment on column public.app_logs.id
    is '日志id';

comment on column public.app_logs.created_at
    is '创建时间';

comment on column public.app_logs.thread
    is '线程id';

comment on column public.app_logs.level
    is '日志级别';

comment on column public.app_logs.logger
    is '记录者';

comment on column public.app_logs.message
    is '日志消息';

comment on column public.app_logs.exception
    is '异常信息';
