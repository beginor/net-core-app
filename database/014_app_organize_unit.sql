-- table: public.app_organize_unit

-- drop table if exists public.app_organize_unit;

create table if not exists public.app_organize_unit
(
    id bigint not null default snow_flake_id(),
    parent_id bigint,
    code character varying(32) collate pg_catalog."default" not null,
    name character varying(32) collate pg_catalog."default" not null,
    description character varying(128) collate pg_catalog."default",
    sequence real not null,
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_app_organize_unit primary key (id),
    constraint fk_app_organize_unit_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_app_organize_unit_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)

tablespace pg_default;

alter table if exists public.app_organize_unit
    owner to postgres;

comment on table public.app_organize_unit
    is '组织单元';

comment on column public.app_organize_unit.id
    is '组织单元id';

comment on column public.app_organize_unit.parent_id
    is '上级组织单元 id';

comment on column public.app_organize_unit.code
    is '组织单元编码';

comment on column public.app_organize_unit.name
    is '组织单元名称';

comment on column public.app_organize_unit.description
    is '组织单元说明';

comment on column public.app_organize_unit.sequence
    is '组织机构排序';

comment on column public.app_organize_unit.creator_id
    is '创建者id';

comment on column public.app_organize_unit.created_at
    is '创建时间';

comment on column public.app_organize_unit.updater_id
    is '更新者id';

comment on column public.app_organize_unit.updated_at
    is '更新时间';

comment on column public.app_organize_unit.is_deleted
    is '是否删除';

alter table public.app_users
    add column organize_unit_id bigint default 0 not null;
comment on column public.app_users.organize_unit_id
    is '组织单元ID'
