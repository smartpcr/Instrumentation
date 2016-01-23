# Instrumentation
instrumentation reference implementation for Azure application

1. Create sample proj with the following layers
	1) DTO obj
	2) Repository Layer
	3) Provider Layer
	4) REST API (Wenb Role)
	5) Queue
	6) Pipeline framework
	7) Worker Role (Workflow - Declarative)

2. Instrumentation
	1) Method boundary: tracking -> specify sink layer (Multicast Aspect)
	2) Performance: ETW+SLAB w/ semantic logging, sink -> RealTime + AzureBlob + ElasticSearch
	3) Caching Provider (3 levels: in-memory, redis/disk, table storage)
	4) Use global flag to turn on/off instrumentation
	5) counters: 
		a) method: total_count, calls_per_hour, timespan_per_call (min, max, avg, total)
		b) layer: 
		c) memory
		d) cpu
		e) disk IO
		f) network IO
		g) exception - correlationId

3. Criteria
	1) able to follow call graph in async context
	2) should not have much overhead
	3) query trace in realtime
	4) ?
