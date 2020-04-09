-- table: public.app_nav_items

-- drop table public.app_nav_items;

create table public.app_nav_items
(
    id bigint not null default public.snow_flake_id(),
    parent_id bigint,
    title character varying(16) collate pg_catalog."default" not null,
    tooltip character varying(64) collate pg_catalog."default",
    icon character varying(32) collate pg_catalog."default",
    url character varying(256) collate pg_catalog."default",
    sequence real,
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    roles character varying(64)[],
    target character varying(16);
    constraint pk_app_nav_items primary key (id),
    constraint fk_app_nav_items_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_feaure_maps_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_nav_items
    owner to postgres;
comment on table public.app_nav_items
    is '导航节点（菜单）';

comment on column public.app_nav_items.id
    is '节点id';

comment on column public.app_nav_items.title
    is '标题';

comment on column public.app_nav_items.tooltip
    is '提示文字';

comment on column public.app_nav_items.url
    is '导航地址';

comment on column public.app_nav_items.creator_id
    is '创建者id';

comment on column public.app_nav_items.created_at
    is '创建时间';

comment on column public.app_nav_items.updater_id
    is '更新者id';

comment on column public.app_nav_items.updated_at
    is '更新时间';

comment on column public.app_nav_items.is_deleted
    is '是否删除';

comment on column public.app_nav_items.sequence
    is '顺序';

comment on column public.app_nav_items.icon
    is '图标';

comment on column public.app_nav_items.roles
    is '能看到该菜单项的角色';

comment on column public.app_nav_items.target
    is '导航目标';
