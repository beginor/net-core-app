-- Table: public.tilemaps

-- DROP TABLE public.tilemaps;

CREATE TABLE IF NOT EXISTS public.tilemaps
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    cache_directory character varying(512) COLLATE pg_catalog."default" NOT NULL,
    map_tile_info_path character varying(512) COLLATE pg_catalog."default" NOT NULL,
    content_type character varying(64) COLLATE pg_catalog."default" NOT NULL,
    is_bundled boolean NOT NULL,
    min_level smallint NOT NULL DEFAULT 0,
    max_level smallint NOT NULL DEFAULT 23,
    creator_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    created_at timestamp without time zone NOT NULL,
    updater_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    updated_at timestamp without time zone NOT NULL,
    is_deleted boolean NOT NULL,
    CONSTRAINT pk_tilemaps PRIMARY KEY (id),
    CONSTRAINT fk_tilemaps_creator FOREIGN KEY (creator_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_tilemaps_updator FOREIGN KEY (updater_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE public.tilemaps
    OWNER to postgres;

COMMENT ON TABLE public.tilemaps
    IS '切片地图';

COMMENT ON COLUMN public.tilemaps.id
    IS '切片地图id';

COMMENT ON COLUMN public.tilemaps.name
    IS '切片地图名称';

COMMENT ON COLUMN public.tilemaps.cache_directory
    IS '缓存目录';

COMMENT ON COLUMN public.tilemaps.map_tile_info_path
    IS '切片信息路径';

COMMENT ON COLUMN public.tilemaps.content_type
    IS '内容类型';

COMMENT ON COLUMN public.tilemaps.is_bundled
    IS '是否为紧凑格式';

COMMENT ON COLUMN public.tilemaps.creator_id
    IS '创建者id';

COMMENT ON COLUMN public.tilemaps.created_at
    IS '创建时间';

COMMENT ON COLUMN public.tilemaps.updater_id
    IS '更新者id';

COMMENT ON COLUMN public.tilemaps.updated_at
    IS '更新时间';

COMMENT ON COLUMN public.tilemaps.is_deleted
    IS '是否删除';

COMMENT ON COLUMN public.tilemaps.min_level
    IS '最小缩放级别';

COMMENT ON COLUMN public.tilemaps.max_level
    IS '最大缩放级别';
