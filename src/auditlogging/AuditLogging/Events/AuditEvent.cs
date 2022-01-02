﻿using AuditLogging.Helpers.Common;

namespace AuditLogging.Events
{
    /// <summary>
    /// Default audit event for logging
    /// </summary>
    public abstract class AuditEvent
    {
        protected AuditEvent()
        {
            Event = GetType().GetNameWithoutGenericParams();
        }

        /// <summary>
        /// Event name
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Source of logging events
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Event category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Identifier of caller which is responsible for the event
        /// </summary>
        public string SubjectIdentifier { get; set; }

        /// <summary>
        /// Name of caller which is responsible for the event
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// Subject Type - User/Machine
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// Subject - some additional data
        /// </summary>
        public object SubjectAdditionalData { get; set; }

        /// <summary>
        /// Information about request/action
        /// </summary>
        public object Action { get; set; }
    }
}