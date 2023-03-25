export interface Project {
  projectId?: string;
  ownerId?: string;
  name?: string;
  domain?: string;
  port?: number;
  containerName?: string;
  proxyId?: number;
  certificateId?: number;
  running?: boolean;
}
