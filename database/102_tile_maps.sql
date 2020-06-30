-- table: public.tile_maps

-- drop table public.tile_maps;

create table public.tile_maps
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    cache_directory character varying(512) collate pg_catalog."default" not null,
    map_tile_info_path character varying(512) collate pg_catalog."default" not null,
    content_type character varying(64) collate pg_catalog."default" not null,
    is_bundled boolean not null,
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_tile_maps primary key (id),
    constraint fk_tile_maps_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_tile_maps_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)
with (
    oids = false
)
tablespace pg_default;

alter table public.tile_maps
    owner to postgres;
comment on table public.tile_maps
    is '切片地图';

comment on column public.tile_maps.id
    is '切片地图id';

comment on column public.tile_maps.name
    is '切片地图名称';

comment on column public.tile_maps.cache_directory
    is '缓存目录';

comment on column public.tile_maps.map_tile_info_path
    is '切片信息路径';

comment on column public.tile_maps.is_bundled
    is '是否为紧凑格式';

comment on column public.tile_maps.creator_id
    is '创建者id';

comment on column public.tile_maps.created_at
    is '创建时间';

comment on column public.tile_maps.updater_id
    is '更新者id';

comment on column public.tile_maps.updated_at
    is '更新时间';

comment on column public.tile_maps.is_deleted
    is '是否删除';

comment on column public.tile_maps.content_type
    is '内容类型';
