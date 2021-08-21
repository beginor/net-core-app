-- table: public.vectortiles

-- drop table public.vectortiles;

create table public.vectortiles
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    directory character varying(512) collate pg_catalog."default" not null,
    min_zoom smallint,
    max_zoom smallint,
    default_style character varying(32),
    min_latitude numeric(10, 8),
    max_latitude numeric(10, 8),
    min_longitude numeric(11, 8),
    max_longitude numeric(11, 8),
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_vectortiles primary key (id),
    constraint fk_vectortiles_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_vectortiles_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
)
with (
    oids = false
)
tablespace pg_default;

alter table public.vectortiles
    owner to postgres;

comment on table public.vectortiles
    is '矢量切片包';

comment on column public.vectortiles.id
    is '矢量切片包id';

comment on column public.vectortiles.name
    is '矢量切片包名称';

comment on column public.vectortiles.directory
    is '矢量切片包目录';

comment on column public.vectortiles.min_zoom
    is '最小缩放级别';

comment on column public.vectortiles.max_zoom
    is '最大缩放级别';

comment on column public.vectortiles.creator_id
    is '创建者id';

comment on column public.vectortiles.created_at
    is '创建时间';

comment on column public.vectortiles.updater_id
    is '更新者id';

comment on column public.vectortiles.updated_at
    is '更新时间';

comment on column public.vectortiles.is_deleted
    is '是否删除';

comment on column public.vectortiles.default_style
    is '默认样式';

comment on column public.vectortiles.min_latitude
    is '最小纬度';

comment on column public.vectortiles.max_latitude
    is '最大纬度';

comment on column public.vectortiles.min_longitude
    is '最小经度';

comment on column public.vectortiles.max_longitude
    is '最大经度';
