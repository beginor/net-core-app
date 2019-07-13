-- Table: public.app_nav_items

-- DROP TABLE public.app_nav_items;

CREATE TABLE public.app_nav_items
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    parent_id bigint,
    title character varying(16) COLLATE pg_catalog."default" NOT NULL,
    tooltip character varying(64) COLLATE pg_catalog."default",
    icon character varying(32) COLLATE pg_catalog."default",
    url character varying(256) COLLATE pg_catalog."default",
    sequence real,
    creator_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    created_at timestamp without time zone NOT NULL,
    updater_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    update_at timestamp without time zone NOT NULL,
    is_deleted boolean NOT NULL,
    CONSTRAINT pk_app_nav_items PRIMARY KEY (id),
    CONSTRAINT fk_app_nav_items_creator FOREIGN KEY (creator_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_feaure_maps_updator FOREIGN KEY (updater_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.app_nav_items
    OWNER to postgres;
COMMENT ON TABLE public.app_nav_items
    IS '导航节点（菜单）';

COMMENT ON COLUMN public.app_nav_items.id
    IS '节点ID';

COMMENT ON COLUMN public.app_nav_items.title
    IS '标题';

COMMENT ON COLUMN public.app_nav_items.tooltip
    IS '提示文字';

COMMENT ON COLUMN public.app_nav_items.url
    IS '导航地址';

COMMENT ON COLUMN public.app_nav_items.creator_id
    IS '创建者ID';

COMMENT ON COLUMN public.app_nav_items.created_at
    IS '创建时间';

COMMENT ON COLUMN public.app_nav_items.updater_id
    IS '更新者ID';

COMMENT ON COLUMN public.app_nav_items.update_at
    IS '更新时间';

COMMENT ON COLUMN public.app_nav_items.is_deleted
    IS '是否删除';

COMMENT ON COLUMN public.app_nav_items.sequence
    IS '顺序';

COMMENT ON COLUMN public.app_nav_items.icon
    IS '图标';

