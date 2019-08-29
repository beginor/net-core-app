-- table: public.app_privileges

-- drop table public.app_privileges;

create table public.app_privileges
(
    id bigint not null default public.snow_flake_id(),
    module character varying(32) collate pg_catalog."default" not null,
    name character varying(64) collate pg_catalog."default" not null,
    description character varying(128) collate pg_catalog."default",
    is_required boolean not null default false,
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

comment on column public.app_privileges.is_required
    is '是否必须。 与代码中的 authorize 标记对应的权限为必须的权限， 否则为可选的。';
