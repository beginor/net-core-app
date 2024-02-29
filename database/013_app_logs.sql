-- Table: public.app_logs

-- DROP TABLE IF EXISTS public.app_logs;

CREATE TABLE IF NOT EXISTS public.app_logs
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    created_at timestamp without time zone NOT NULL,
    thread character varying(8) COLLATE pg_catalog."default" NOT NULL,
    level character varying(16) COLLATE pg_catalog."default" NOT NULL,
    logger character varying(256) COLLATE pg_catalog."default",
    message character varying(4096) COLLATE pg_catalog."default",
    exception character varying(4096) COLLATE pg_catalog."default",
    CONSTRAINT pk_app_logs PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_logs
    OWNER to postgres;

COMMENT ON TABLE public.app_logs
    IS '应用程序日志';

COMMENT ON COLUMN public.app_logs.id
    IS '日志ID';

COMMENT ON COLUMN public.app_logs.created_at
    IS '创建时间';

COMMENT ON COLUMN public.app_logs.thread
    IS '线程ID';

COMMENT ON COLUMN public.app_logs.level
    IS '日志级别';

COMMENT ON COLUMN public.app_logs.logger
    IS '记录者';

COMMENT ON COLUMN public.app_logs.message
    IS '日志消息';

COMMENT ON COLUMN public.app_logs.exception
    IS '异常信息';
