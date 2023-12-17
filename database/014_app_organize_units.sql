-- table: public.app_organize_units

-- drop table if exists public.app_organize_units;

create table if not exists public.app_organize_units
(
    id bigint not null default snow_flake_id(),
    parent_id bigint,
    code character varying(512) collate pg_catalog."default",
    name character varying(32) collate pg_catalog."default" not null,
    description character varying(128) collate pg_catalog."default",
    sequence real not null,
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_app_organize_units primary key (id),
    constraint fk_app_organize_units_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_app_organize_units_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)

tablespace pg_default;

alter table if exists public.app_organize_units
    owner to postgres;

comment on table public.app_organize_units
    is '组织单元';

comment on column public.app_organize_units.id
    is '组织单元id';

comment on column public.app_organize_units.parent_id
    is '上级组织单元 id';

comment on column public.app_organize_units.code
    is '组织单元编码';

comment on column public.app_organize_units.name
    is '组织单元名称';

comment on column public.app_organize_units.description
    is '组织单元说明';

comment on column public.app_organize_units.sequence
    is '组织机构排序';

comment on column public.app_organize_units.creator_id
    is '创建者id';

comment on column public.app_organize_units.created_at
    is '创建时间';

comment on column public.app_organize_units.updater_id
    is '更新者id';

comment on column public.app_organize_units.updated_at
    is '更新时间';

comment on column public.app_organize_units.is_deleted
    is '是否删除';

alter table public.app_users
    add column organize_unit_id bigint default 0 not null;
comment on column public.app_users.organize_unit_id
    is '组织单元ID'

alter table if exists public.app_users
    add constraint fk_app_users_organize_unit foreign key (organize_unit_id)
    references public.app_organize_units (id) match simple
    on update no action
    on delete no action
    not valid;
