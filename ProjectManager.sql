CREATE TABLE `Projects` (
  `projectId` varchar(36) NOT NULL,
  `ownerId` varchar(36) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `port` int(5) DEFAULT NULL,
  `containerName` varchar(255) DEFAULT NULL,
  `proxyId` int(30) DEFAULT NULL,
  `certificateId` int(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Tokens` (
  `tokenId` varchar(36) NOT NULL,
  `userId` varchar(36) DEFAULT NULL,
  `clientIp` varchar(255) DEFAULT NULL,
  `created` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `Users` (
  `userId` varchar(36) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `username` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `maxProjects` int(3) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

ALTER TABLE `Projects`
  ADD PRIMARY KEY (`projectId`);

ALTER TABLE `Tokens`
  ADD PRIMARY KEY (`tokenId`);

ALTER TABLE `Users`
  ADD PRIMARY KEY (`userId`);
COMMIT;
