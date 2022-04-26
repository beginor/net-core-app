-- table: public.base_resources

-- drop table if exists public.base_resources;

create table if not exists public.base_resources
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default",
    category_id bigint not null,
    type character varying(64) collate pg_catalog."default" not null,
    tags character varying(32)[] collate pg_catalog."default",
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_base_resources primary key (id),
    constraint fk_base_resources_category foreign key (category_id)
        references public.categories (id) match simple
        on update no action
        on delete no action
        not valid,
    constraint fk_base_resources_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_base_resources_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)

tablespace pg_default;

alter table if exists public.base_resources
    owner to postgres;

comment on table public.base_resources
    is '数据资源的基类';

comment on column public.base_resources.id
    is '资源id';

comment on column public.base_resources.name
    is '资源名称';

comment on column public.base_resources.type
    is '资源类型';

comment on column public.base_resources.tags
    is '资源标签';

comment on column public.base_resources.category_id
    is '资源类别id';

comment on column public.base_resources.creator_id
    is '创建者id';

comment on column public.base_resources.created_at
    is '创建时间';

comment on column public.base_resources.updater_id
    is '更新者id';

comment on column public.base_resources.updated_at
    is '更新时间';

comment on column public.base_resources.is_deleted
    is '是否删除';
