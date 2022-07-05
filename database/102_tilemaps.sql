-- table: public.tilemaps

-- drop table public.tilemaps;

create table if not exists public.tilemaps
(
    id bigint not null default snow_flake_id(),
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
    constraint pk_tilemaps primary key (id)
)

tablespace pg_default;

alter table public.tilemaps
    owner to postgres;

comment on table public.tilemaps
    is '切片地图';

comment on column public.tilemaps.id
    is '切片地图id';

comment on column public.tilemaps.cache_directory
    is '缓存目录';

comment on column public.tilemaps.map_tile_info_path
    is '切片信息路径';

comment on column public.tilemaps.content_type
    is '内容类型';

comment on column public.tilemaps.is_bundled
    is '是否为紧凑格式';

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
