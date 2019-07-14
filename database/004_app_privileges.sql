-- table: public.app_privileges

-- drop table public.app_privileges;

create table public.app_privileges
(
    id bigint not null,
    module character varying(32) collate pg_catalog."default" not null,
    name character varying(64) collate pg_catalog."default" not null,
    description character varying(128) collate pg_catalog."default",
    is_optional boolean not null default false,
    constraint pk_app_privileges primary key (id),
    constraint uk_app_privileges_name unique (name)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_privileges
    owner to postgres;
comment on table public.app_privileges
    is '系统权限';

comment on column public.app_privileges.id
    is '权限id';

comment on column public.app_privileges.module
    is '权限模块';

comment on column public.app_privileges.name
    is '权限名称( identity 的策略名称)';

comment on column public.app_privileges.description
    is '权限描述';

comment on column public.app_privileges.is_optional
    is '是否可选。 与代码中的 authorize 标记对应的为必须的权限， 否则为可选的。';
