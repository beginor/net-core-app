-- table: public.tilemaps

-- drop table public.tilemaps;

create table if not exists public.tilemaps
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    cache_directory character varying(512) collate pg_catalog."default" not null,
    map_tile_info_path character varying(512) collate pg_catalog."default" not null,
    folder_structure character varying(16) collate pg_catalog."default" not null,
    content_type character varying(64) collate pg_catalog."default" not null,
    is_bundled boolean not null,
    min_level smallint not null default 0,
    max_level smallint not null default 23,
    min_latitude numeric(10, 8),
    max_latitude numeric(10, 8),
    min_longitude numeric(11, 8),
    max_longitude numeric(11, 8),
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_tilemaps primary key (id),
    constraint fk_tilemaps_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_tilemaps_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)

tablespace pg_default;

alter table public.tilemaps
    owner to postgres;

comment on table public.tilemaps
    is '切片地图';

comment on column public.tilemaps.id
    is '切片地图id';

comment on column public.tilemaps.name
    is '切片地图名称';

comment on column public.tilemaps.cache_directory
    is '缓存目录';

comment on column public.tilemaps.map_tile_info_path
    is '切片信息路径';

comment on column public.tilemaps.content_type
    is '内容类型';

comment on column public.tilemaps.is_bundled
    is '是否为紧凑格式';

comment on column public.tilemaps.creator_id
    is '创建者id';

comment on column public.tilemaps.created_at
    is '创建时间';

comment on column public.tilemaps.updater_id
    is '更新者id';

comment on column public.tilemaps.updated_at
    is '更新时间';

comment on column public.tilemaps.is_deleted
    is '是否删除';

comment on column public.tilemaps.min_level
    is '最小缩放级别';

comment on column public.tilemaps.max_level
    is '最大缩放级别';

comment on column public.tilemaps.min_latitude
    is '最小纬度';

comment on column public.tilemaps.max_latitude
    is '最大纬度';

comment on column public.tilemaps.min_longitude
    is '最小经度';

comment on column public.tilemaps.max_longitude
    is '最大经度';

comment on column public.tilemaps.folder_structure
    is '目录结构';
