-- table: public.app_client_errors

-- drop table public.app_client_errors;

create table public.app_client_errors
(
    id bigint not null default snow_flake_id(),
    user_name character varying(64) collate pg_catalog."default" not null,
    occured_at timestamp with time zone not null,
    user_agent character varying(512) collate pg_catalog."default" not null,
    path character varying(1024) collate pg_catalog."default",
    message character varying(2048) collate pg_catalog."default",
    constraint app_client_errors_pkey primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_client_errors
    owner to postgres;
comment on table public.app_client_errors
    is '客户端错误记录';

comment on column public.app_client_errors.id
    is '客户端错误记录ID';

comment on column public.app_client_errors.user_name
    is '用户名';

comment on column public.app_client_errors.occured_at
    is '错误发生时间';

comment on column public.app_client_errors.user_agent
    is '用户浏览器代理';

comment on column public.app_client_errors.path
    is '错误发生的路径地址';

comment on column public.app_client_errors.message
    is '错误消息';

create index idx_app_client_errors_occured_at
    on public.app_client_errors using btree
    (occured_at desc nulls last)
    tablespace pg_default;
