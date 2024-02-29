-- Table: public.app_audit_logs

-- DROP TABLE IF EXISTS public.app_audit_logs;

CREATE TABLE IF NOT EXISTS public.app_audit_logs
(
    id bigint NOT NULL DEFAULT nextval('snow_flake_id_seq'::regclass),
    request_path character varying(256) COLLATE pg_catalog."default" NOT NULL,
    request_method character varying(8) COLLATE pg_catalog."default" NOT NULL,
    user_name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    start_at timestamp without time zone NOT NULL,
    duration double precision NOT NULL,
    response_code integer NOT NULL,
    controller_name character varying(64) COLLATE pg_catalog."default",
    action_name character varying(64) COLLATE pg_catalog."default",
    description character varying(256) COLLATE pg_catalog."default",
    ip character varying(64) COLLATE pg_catalog."default",
    host_name character varying(32) COLLATE pg_catalog."default",
    CONSTRAINT pk_app_audit_logs PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_audit_logs
    OWNER to postgres;

COMMENT ON TABLE public.app_audit_logs
    IS '审计日志';

COMMENT ON COLUMN public.app_audit_logs.id
    IS '审计日志ID';

COMMENT ON COLUMN public.app_audit_logs.request_path
    IS '请求路径';

COMMENT ON COLUMN public.app_audit_logs.request_method
    IS '请求方法';

COMMENT ON COLUMN public.app_audit_logs.user_name
    IS '用户名';

COMMENT ON COLUMN public.app_audit_logs.start_at
    IS '开始时间';

COMMENT ON COLUMN public.app_audit_logs.duration
    IS '耗时(毫秒)';

COMMENT ON COLUMN public.app_audit_logs.response_code
    IS '响应状态码';

COMMENT ON COLUMN public.app_audit_logs.controller_name
    IS '控制器名称';

COMMENT ON COLUMN public.app_audit_logs.action_name
    IS '动作名称';

COMMENT ON COLUMN public.app_audit_logs.description
    IS '描述';

COMMENT ON COLUMN public.app_audit_logs.ip
    IS '客户端IP地址';

COMMENT ON COLUMN public.app_audit_logs.host_name
    IS '请求的主机名';
