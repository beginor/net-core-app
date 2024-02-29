-- Table: public.app_nav_items

-- DROP TABLE IF EXISTS public.app_nav_items;

CREATE TABLE IF NOT EXISTS public.app_nav_items
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
    updated_at timestamp without time zone NOT NULL,
    is_deleted boolean NOT NULL,
    roles character varying(64)[] COLLATE pg_catalog."default",
    target character varying(16) COLLATE pg_catalog."default" DEFAULT ''::character varying,
    frame_url character varying(256) COLLATE pg_catalog."default",
    is_hidden boolean,
    CONSTRAINT pk_app_nav_items PRIMARY KEY (id),
    CONSTRAINT fk_app_nav_items_creator FOREIGN KEY (creator_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_app_navitems_updator FOREIGN KEY (updater_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_nav_items
    OWNER to postgres;

COMMENT ON TABLE public.app_nav_items
    IS '导航节点（菜单）';

COMMENT ON COLUMN public.app_nav_items.id
    IS '节点ID';

COMMENT ON COLUMN public.app_nav_items.parent_id
    IS '父节点ID';

COMMENT ON COLUMN public.app_nav_items.title
    IS '标题';

COMMENT ON COLUMN public.app_nav_items.tooltip
    IS '提示文字';

COMMENT ON COLUMN public.app_nav_items.icon
    IS '图标';

COMMENT ON COLUMN public.app_nav_items.url
    IS '导航地址';

COMMENT ON COLUMN public.app_nav_items.sequence
    IS '顺序';

COMMENT ON COLUMN public.app_nav_items.creator_id
    IS '创建者ID';

COMMENT ON COLUMN public.app_nav_items.created_at
    IS '创建时间';

COMMENT ON COLUMN public.app_nav_items.updater_id
    IS '更新者ID';

COMMENT ON COLUMN public.app_nav_items.updated_at
    IS '更新时间';

COMMENT ON COLUMN public.app_nav_items.is_deleted
    IS '是否删除';

COMMENT ON COLUMN public.app_nav_items.roles
    IS '能看到该菜单项的角色';

COMMENT ON COLUMN public.app_nav_items.target
    IS '导航目标';

COMMENT ON COLUMN public.app_nav_items.frame_url
    IS '内嵌窗口地址';

COMMENT ON COLUMN public.app_nav_items.is_hidden
    IS '是否隐藏';
